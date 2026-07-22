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

public interface IElasticTodoSearchService
{
    Task IndexTodoAsync(Guid todoId, CancellationToken cancellationToken = default);
    Task RebuildTodoIndexAsync(CancellationToken cancellationToken = default);
    Task<List<TodoDocument>> SearchTodosAsync(Guid userId, string text, int limit, CancellationToken cancellationToken = default);
}

internal sealed class ElasticTodoSearchService : IElasticTodoSearchService
{
    private readonly ElasticsearchClient client;
    private readonly ILogger<ElasticTodoSearchService> logger;
    private readonly IApplicationDbContext applicationDbContext;

    public ElasticTodoSearchService(ElasticsearchClient client, ILogger<ElasticTodoSearchService> logger, IApplicationDbContext applicationDbContext)
    {
        this.client = client;
        this.logger = logger;
        this.applicationDbContext = applicationDbContext;
    }

    public async Task IndexTodoAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        global::Elastic.Clients.Elasticsearch.IndexManagement.ExistsResponse exists = await client.Indices.ExistsAsync(ElasticSearchConstants.ElasticTodoIndex, cancellationToken);

        if (!exists.Exists)
        {
            CreateIndexResponse indexResponse = await client.Indices.CreateAsync(ElasticSearchConstants.ElasticTodoIndex, cancellationToken);

            if (!indexResponse.IsValidResponse)
            {
                logger.LogError("Failed to index document: {Reason}", indexResponse.ElasticsearchServerError?.Error?.Reason);
            }

            logger.LogInformation("Created Elasticsearch index 'todos'.");
        }

        TodoItem? todo = await applicationDbContext.TodoItems
            .AsNoTracking()
            .Include(t => t.SubItems)
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(
                t => t.Id == todoId,
                cancellationToken);

        if (todo is null)
        {
            return;
        }

        TodoDocument document = CreateDocument(todo);

        IndexResponse response = await client.IndexAsync(
            document,
            i => i.Index(ElasticSearchConstants.ElasticTodoIndex),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to index document: {Reason}", response.ElasticsearchServerError?.Error?.Reason);
        }
    }

    public async Task RebuildTodoIndexAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Rebuilding Elasticsearch index...");

        if ((await client.Indices.ExistsAsync(ElasticSearchConstants.ElasticTodoIndex, cancellationToken)).Exists)
        {
            await client.Indices.DeleteAsync(ElasticSearchConstants.ElasticTodoIndex, cancellationToken);
        }

        CreateIndexResponse createResponse = await client.Indices.CreateAsync(ElasticSearchConstants.ElasticTodoIndex, cancellationToken);

        if (!createResponse.IsValidResponse)
        {
            throw new InvalidOperationException($"Failed to create Elasticsearch index: {createResponse.ElasticsearchServerError?.Error?.Reason}");
        }

        int totalTodos = await applicationDbContext.TodoItems.CountAsync(cancellationToken);

        logger.LogInformation("Indexing {Count} todos...", totalTodos);

        for (int skip = 0; skip < totalTodos; skip += ElasticSearchConstants.BatchSize)
        {
            List<TodoItem> todos = await applicationDbContext.TodoItems
                .AsNoTracking()
                .Include(t => t.SubItems)
                .Include(t => t.Attachments)
                .OrderBy(t => t.Id)
                .Skip(skip)
                .Take(ElasticSearchConstants.BatchSize)
                .ToListAsync(cancellationToken);

            BulkRequest bulkRequest = new(ElasticSearchConstants.ElasticTodoIndex)
            {
                Operations = todos
                    .Select(CreateDocument)
                    .Select(d => new BulkIndexOperation<TodoDocument>(d))
                    .Cast<IBulkOperation>()
                    .ToList()
            };

            BulkResponse response = await client.BulkAsync(bulkRequest, cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new InvalidOperationException(
                    $"Bulk indexing failed. {response.ElasticsearchServerError?.Error?.Reason}");
            }

            logger.LogInformation("Indexed {Indexed}/{Total} todos.", Math.Min(skip + todos.Count, totalTodos), totalTodos);
        }

        logger.LogInformation("Elasticsearch rebuild finished.");
    }

    public async Task<List<TodoDocument>> SearchTodosAsync(Guid userId, string text, int limit, CancellationToken cancellationToken = default)
    {
        var wildcardQueries = ElasticSearchConstants.TodoFields
            .Select(field => (Query)new WildcardQuery
            {
                Field = new Field(field),
                Value = $"*{text}*"
            })
            .ToList();

        SearchResponse<TodoDocument> response =
            await client.SearchAsync<TodoDocument>(s => s
                .Indices(ElasticSearchConstants.ElasticTodoIndex)
                .Size(limit)
                .Query(q => q
                    .Bool(b => b
                        .Filter(f => f
                            .Term(t => t
                                .Field(ElasticSearchConstants.UserIdKeyword)
                                .Value(userId.ToString())))
                        .Must(m => m
                            .Bool(bb => bb
                                .Should(wildcardQueries)
                                .MinimumShouldMatch(1)
                            ))
                    )),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Elasticsearch search failed: {Reason}", response.ElasticsearchServerError?.Error?.Reason);

            return [];
        }

        return [.. response.Documents];
    }

    private static TodoDocument CreateDocument(TodoItem todo)
    {
        return new TodoDocument
        {
            Id = todo.Id,
            UserId = todo.UserId,
            Description = todo.Description,
            Priority = (int)todo.Priority,
            PriorityAsText = todo.Priority.ToString(),
            DueDate = todo.DueDate,
            IsCompleted = todo.IsCompleted,
            CreatedOn = todo.CreatedOn,
            UpdatedOn = todo.UpdatedOn,

            Subtasks =
            [
                .. todo.SubItems.Select(s => new TodoSubtaskDocument
            {
                Id = s.Id,
                Description = s.Description,
                IsCompleted = s.IsCompleted,
                CompletedAt = s.CompletedAt,
                Order = s.Order
            })
            ],

            Attachments =
            [
                .. todo.Attachments.Select(a => new TodoAttachmentDocument
            {
                Id = a.Id,
                OriginalFileName = a.OriginalFileName,
                ContentType = a.ContentType,
                Size = a.Size
            })
            ]
        };
    }
}


