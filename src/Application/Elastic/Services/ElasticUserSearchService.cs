using Application.Common.Interfaces;
using Application.Elastic.Constants;
using Application.Elastic.Documents;
using Application.Elastic.Mappings;
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
    Task RebuildUserIndexAsync(CancellationToken cancellationToken = default);
    Task<List<UserSearchDocument>> SearchUsersAsync(string text, int limit, CancellationToken cancellationToken = default);
    Task<UserSearchDocument?> GetUserAsync(Guid id, CancellationToken cancellationToken);
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

    #region Index

    public async Task IndexUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (!(await client.Indices.ExistsAsync(ElasticSearchConstants.ElasticUserIndex, cancellationToken)).Exists)
        {
            CreateIndexResponse createResponse = await client.Indices.CreateAsync(UserIndexMappings.Create(), cancellationToken);

            if (!createResponse.IsValidResponse)
            {
                throw new InvalidOperationException($"Failed to create Elasticsearch index: {createResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            logger.LogInformation("Created Elasticsearch index 'users'.");
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
            Email = user.Email,
            CreatedOn = user.CreatedOn,
            UpdatedOn = user.UpdatedOn
        };

        await client.IndexAsync(
            document,
            i => i.Index("users"),
            cancellationToken);
    }

    public async Task RebuildUserIndexAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Rebuilding Elasticsearch index...");

        if ((await client.Indices.ExistsAsync(ElasticSearchConstants.ElasticUserIndex, cancellationToken)).Exists)
        {
            await client.Indices.DeleteAsync(ElasticSearchConstants.ElasticUserIndex, cancellationToken);

            CreateIndexResponse createResponse = await client.Indices.CreateAsync(UserIndexMappings.Create(), cancellationToken);

            if (!createResponse.IsValidResponse)
            {
                throw new InvalidOperationException($"Failed to create Elasticsearch index: {createResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            logger.LogInformation("Created Elasticsearch index 'users'.");
        }

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
                        Email = x.Email,
                        UpdatedOn = x.UpdatedOn,
                        CreatedOn = x.CreatedOn
                    })
                    .ToListAsync(cancellationToken);

            await client.BulkAsync(b => b
                .Index(ElasticSearchConstants.ElasticUserIndex)
                .IndexMany(batch),
                cancellationToken);
        }
    }

    #endregion

    #region Queries
    
    public async Task<UserSearchDocument?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        SearchResponse<UserSearchDocument> response =
            await client.SearchAsync<UserSearchDocument>(s => s
                .Indices(ElasticSearchConstants.ElasticUserIndex)
                .Size(1)
                .Query(q => q
                    .Bool(b => b
                        .Filter(
                            f => f.Term(t => t
                                .Field(ElasticSearchConstants.Id)
                                .Value(id.ToString()))
                        )
                    )
                ),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Elasticsearch search failed: {Reason}", response.ElasticsearchServerError?.Error?.Reason);

            return null;
        }

        return response.Documents.SingleOrDefault();
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
                .Indices(ElasticSearchConstants.ElasticUserIndex)
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
    
    #endregion

}


