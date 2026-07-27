using System.Linq.Dynamic.Core;
using Application.Common.Interfaces;
using Application.Elastic.Constants;
using Application.Elastic.Documents;
using Application.Elastic.Extensions;
using Application.Elastic.Mappings;
using Application.Todos.Get;
using Domain.API;
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
    Task<List<TodoDocument>> GetQuickSearchDetailAsync(Guid userId, string text, int limit, CancellationToken cancellationToken = default);
    Task<PagedResponse<TodoDocument>> SearchTodosAsync(Guid userId, TodoFilter? filter, Sorted? sorting, Paginated? pagination, CancellationToken cancellationToken = default);
    Task<TodoDocument?> GetSearchDetailAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<PagedResponse<TodoDocument>> SemanticSearchTodosAsync(Guid userId, float[] embedding, int page, int size, Sorted? sorting, int candidateLimit, CancellationToken cancellationToken);
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

    #region Index

    public async Task IndexTodoAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        if (!(await client.Indices.ExistsAsync(ElasticSearchConstants.ElasticTodoIndex, cancellationToken)).Exists)
        {
            CreateIndexResponse createResponse = await client.Indices.CreateAsync(TodoIndexMappings.Create(), cancellationToken);

            if (!createResponse.IsValidResponse)
            {
                throw new InvalidOperationException($"Failed to create Elasticsearch index: {createResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            logger.LogInformation("Created Elasticsearch index 'todos'.");
        }

        TodoItem? todo = await applicationDbContext.TodoItems
            .AsNoTracking()
            .Include(t => t.SubItems)
            .Include(t => t.Attachments)
            .Include(t => t.Dependencies)
            .Include(t => t.Attachments)
            .Include(t => t.TaskActivities)
            .SingleOrDefaultAsync(t => t.Id == todoId, cancellationToken);

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

            CreateIndexResponse createResponse = await client.Indices.CreateAsync(TodoIndexMappings.Create(), cancellationToken);

            if (!createResponse.IsValidResponse)
            {
                throw new InvalidOperationException($"Failed to create Elasticsearch index: {createResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            logger.LogInformation("Created Elasticsearch index 'todos'.");
        }

        int totalTodos = await applicationDbContext.TodoItems.CountAsync(cancellationToken);

        logger.LogInformation("Indexing {Count} todos...", totalTodos);

        for (int skip = 0; skip < totalTodos; skip += ElasticSearchConstants.BatchSize)
        {
            List<TodoItem> todos = await applicationDbContext.TodoItems
                .AsNoTracking()
                .Include(t => t.SubItems)
                .Include(t => t.Attachments)
                .Include(t => t.TaskActivities)
                .Include(t => t.Dependencies)
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
                throw new InvalidOperationException($"Bulk indexing failed. {response.ElasticsearchServerError?.Error?.Reason}");
            }

            logger.LogInformation("Indexed {Indexed}/{Total} todos.", Math.Min(skip + todos.Count, totalTodos), totalTodos);
        }

        logger.LogInformation("Elasticsearch rebuild finished.");
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
            Embedding = todo.Embedding!.ToArray(),
            IsCompleted = todo.IsCompleted,
            CreatedOn = todo.CreatedOn,
            UpdatedOn = todo.UpdatedOn,
            CompletedOn = todo.CompletedOn,
            Labels = [.. todo.Labels],
            Categories = [.. todo.Categories],
            Subtasks =
            [
                .. todo.SubItems.Select(s => new TodoSubtaskDocument
            {
                Id = s.Id,
                SubItemId = s.TodoItemId,
                Description = s.Description,
                IsCompleted = s.IsCompleted,
                CompletedOn = s.CompletedOn,
                CreatedOn = s.CreatedOn,
                UpdatedOn = s.UpdatedOn,
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
            ],
            Activities =
            [
                ..todo.TaskActivities.Select(a => new TodoActivityDocument
                {
                    ActivityType = (int)a.ActivityType,
                    ActivityTypeAsText = a.ActivityType.ToString(),
                    Description = a.Description,
                    Id = a.Id,
                    Metadata = a.Metadata
                })
            ]
        };
    }
    
    #endregion

    #region Queries
    
    public async Task<TodoDocument?> GetSearchDetailAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        SearchResponse<TodoDocument> response =
            await client.SearchAsync<TodoDocument>(s => s
                .Indices(ElasticSearchConstants.ElasticTodoIndex)
                .Size(1)
                .Query(q => q
                    .Bool(b => b
                        .Filter(
                            f => f.Term(t => t
                                .Field(ElasticSearchConstants.UserId)
                                .Value(userId.ToString())),
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

    public async Task<List<TodoDocument>> GetQuickSearchDetailAsync(Guid userId, string text, int limit, CancellationToken cancellationToken = default)
    {
        List<Query> wildcardQueries =
        [
            .. ElasticSearchConstants.TodoFields.Select(field =>
            (Query)new WildcardQuery
            {
                Field = field,
                Value = $"*{text}*"
            })
        ];

        Query query = new BoolQuery
        {
            Filter =
            [
                new TermQuery
            {
                Field = ElasticSearchConstants.UserId,
                Value = userId.ToString()
            }
            ],

            Must =
            [
                new BoolQuery
            {
                Should = wildcardQueries,
                MinimumShouldMatch = 1
            }
            ]
        };


        SearchResponse<TodoDocument> response =
            await client.SearchAsync<TodoDocument>(
                new SearchRequest<TodoDocument>(
                    ElasticSearchConstants.ElasticTodoIndex)
                {
                    Query = query,
                    Size = limit
                },
                cancellationToken);


        if (!response.IsValidResponse)
        {
            logger.LogError(
                "Elasticsearch search failed: {Reason}",
                response.ElasticsearchServerError?.Error?.Reason);

            return [];
        }

        return [.. response.Documents];
    }

    public async Task<PagedResponse<TodoDocument>> SemanticSearchTodosAsync(Guid userId, float[] embedding, int page, int size, Sorted? sorting, int candidateLimit, CancellationToken cancellationToken)
    {
        SearchResponse<TodoDocument> response =
            await client.SearchAsync<TodoDocument>(
                s => s
                    .Indices(ElasticSearchConstants.ElasticTodoIndex)
                    .Knn(k => k
                        .Field(f => f.Embedding)
                        .QueryVector(embedding)
                        .K(candidateLimit)
                        .NumCandidates(candidateLimit)
                        .Filter(f => f
                            .Term(t => t
                                .Field(ElasticSearchConstants.UserId)
                                .Value(userId.ToString()))))
                    .Size(candidateLimit),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError(
                "Semantic search failed: {Reason}",
                response.ElasticsearchServerError?.Error?.Reason);

            return new PagedResponse<TodoDocument>([], 0);
        }

        IEnumerable<TodoDocument> todos =
            response.Documents;

        //Sorting
        if (sorting is not null && !string.IsNullOrWhiteSpace(sorting.PropertyName))
        {
            string direction = sorting.Descending
                ? "descending"
                : "ascending";

            todos = todos.AsQueryable().OrderBy($"{ElasticFieldName.FromProperty(sorting.PropertyName)} {direction}");
        }

        int totalItems = todos.Count();

        List<TodoDocument> items =
        [
            .. todos
            .Skip((page - 1) * size)
            .Take(size)
        ];

        return new PagedResponse<TodoDocument>(
            items,
            totalItems);
    }

    public async Task<PagedResponse<TodoDocument>> SearchTodosAsync(Guid userId, TodoFilter? filter, Sorted? sorting, Paginated? pagination, CancellationToken cancellationToken = default)
    {
        List<Query> filters =
        [
            new TermQuery
        {
            Field = ElasticSearchConstants.UserId,
            Value = userId.ToString()
        }
        ];

        if (filter is not null)
        {
            if (filter.Priority.HasValue)
            {
                filters.Add(
                    new TermQuery
                    {
                        Field = "priority",
                        Value = (int)filter.Priority
                    });
            }

            if (filter.IsCompleted.HasValue)
            {
                filters.Add(
                    new TermQuery
                    {
                        Field = "isCompleted",
                        Value = filter.IsCompleted.Value
                    });
            }

            if (filter.DueDateFrom.HasValue || filter.DueDateTo.HasValue)
            {
                filters.Add(
                    new DateRangeQuery
                    {
                        Field = "dueDate",
                        Gte = filter.DueDateFrom.HasValue
                            ? filter.DueDateFrom.Value.ToDateTime(
                                TimeOnly.MinValue,
                                DateTimeKind.Utc)
                            : null,

                        Lt = filter.DueDateTo.HasValue
                            ? filter.DueDateTo.Value.ToDateTime(
                                TimeOnly.MinValue,
                                DateTimeKind.Utc)
                            : null
                    });
            }

            if (filter.Categories is { Count: > 0 })
            {
                filters.Add(
                    new TermsQuery
                    {
                        Field = "categories",
                        Terms = new TermsQueryField([.. filter.Categories.Select(FieldValue.String)])
                    });
            }

            if (filter.Labels is { Count: > 0 })
            {
                filters.Add(
                    new TermsQuery
                    {
                        Field = "labels",
                        Terms = new TermsQueryField([.. filter.Labels.Select(FieldValue.String)])
                    });
            }
        }


        Query query = new BoolQuery
        {
            Filter = filters
        };


        SortOptions sortOptions;

        if (sorting is null ||
            string.IsNullOrWhiteSpace(sorting.PropertyName))
        {
            sortOptions = new FieldSort
            {
                Field = "createdOn",
                Order = SortOrder.Desc
            };
        }
        else
        {
            sortOptions = new FieldSort
            {
                Field = ElasticFieldName.FromProperty(sorting.PropertyName),
                Order = sorting.Descending
                    ? SortOrder.Desc
                    : SortOrder.Asc
            };
        }


        SearchResponse<TodoDocument> response =
            await client.SearchAsync<TodoDocument>(
                new SearchRequest<TodoDocument>(
                    ElasticSearchConstants.ElasticTodoIndex)
                {
                    Query = query,

                    TrackTotalHits = true,

                    From = pagination is null
                        ? 0
                        : (pagination.Page - 1) * pagination.Size,

                    Size = pagination?.Size ?? 10,

                    Sort =
                    [
                        sortOptions
                    ]
                },
                cancellationToken);


        if (!response.IsValidResponse)
        {
            logger.LogError("Elasticsearch search failed: {Reason}", response.ElasticsearchServerError?.Error?.Reason);

            return new PagedResponse<TodoDocument>([], 0);
        }

        return new PagedResponse<TodoDocument>([.. response.Documents],(int)response.Total);
    }
    
    #endregion
}
