using Application.Common.Interfaces;
using Application.Elastic.Constants;
using Application.Elastic.Documents;
using Domain.Todos;
using Domain.Users;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Elastic.Services;

public interface IElasticUserSearchService
{
    Task IndexUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RebuildUsersAsync(CancellationToken cancellationToken = default);
    Task<List<UserSearchDocument>> SearchUsersAsync(string text, int limit, CancellationToken cancellationToken = default);
}

internal sealed class ElasticUserSearchService : IElasticUserSearchService
{
    private readonly ElasticsearchClient client;
    private readonly ILogger<ElasticUserSearchService> logger;
    private readonly IApplicationDbContext applicationDbContext;

    public ElasticUserSearchService(ElasticsearchClient client, ILogger<ElasticUserSearchService> logger, IApplicationDbContext applicationDbContext)
    {
        this.client = client;
        this.logger = logger;
        this.applicationDbContext = applicationDbContext;
    }

    public async Task IndexUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        global::Elastic.Clients.Elasticsearch.IndexManagement.ExistsResponse exists = await client.Indices.ExistsAsync(ElasticSearchConstants.ElasticUsersIndex, cancellationToken);

        if (!exists.Exists)
        {
            CreateIndexResponse indexResponse = await client.Indices.CreateAsync(ElasticSearchConstants.ElasticUsersIndex, cancellationToken);

            if (!indexResponse.IsValidResponse)
            {
                logger.LogError("Failed to index document: {Reason}", indexResponse.ElasticsearchServerError?.Error?.Reason);
            }

            logger.LogInformation("Created Elasticsearch index 'todos'.");
        }

        User? user = await applicationDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken);

        if (user is null)
        {
            return;
        }

        UserSearchDocument document = new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };

        await client.IndexAsync(
            document,
            i => i.Index("users"),
            cancellationToken);
    }

    public async Task RebuildUsersAsync(CancellationToken cancellationToken = default)
    {
        int total = await applicationDbContext.Users.CountAsync(cancellationToken);

        for (int skip = 0; skip < total; skip += ElasticSearchConstants.BatchSize)
        {
            List<UserSearchDocument> batch =
                await applicationDbContext.Users
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Skip(skip)
                    .Take(ElasticSearchConstants.BatchSize)
                    .Select(x => new UserSearchDocument
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email
                    })
                    .ToListAsync(cancellationToken);

            await client.BulkAsync(b => b
                .Index(ElasticSearchConstants.ElasticUsersIndex)
                .IndexMany(batch),
                cancellationToken);
        }
    }

    public async Task<List<UserSearchDocument>> SearchUsersAsync(string text, int limit, CancellationToken cancellationToken = default)
    {
        var wildcardQueries = ElasticSearchConstants.UserFields
            .Select(field => (Query)new WildcardQuery
            {
                Field = new Field(field),
                Value = $"*{text}*"
            })
            .ToList();

        SearchResponse<UserSearchDocument> response =
            await client.SearchAsync<UserSearchDocument>(s => s
                .Indices(ElasticSearchConstants.ElasticUsersIndex)
                .Size(limit)
                .Query(q => q
                    .Bool(b => b
                        .Should(wildcardQueries)
                        .MinimumShouldMatch(1)
                    )),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("User search failed: {Reason}", response.ElasticsearchServerError?.Error?.Reason);

            return [];
        }

        return [.. response.Documents];
    }
}


