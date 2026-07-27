using Application.Common.Interfaces;
using Application.Elastic;
using Application.Elastic.Documents;
using Application.Elastic.Services;
using Application.OpenAI.Embeddings;
using Domain.API;
using Domain.Todos;
using Domain.Users;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Search;

internal sealed class SearchTodosQueryHandler(IElasticTodoSearchService elasticTodoSearchService, IEmbeddingsService embeddingsService)
    : IQueryHandler<SearchTodoItemsQuery, PagedResponse<SearchTodoItemResponse>>
{
    private const int SemanticCandidateLimit = 1000;

    public async Task<Result<PagedResponse<SearchTodoItemResponse>>> Handle(
        SearchTodoItemsQuery query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Searchtext))
        {
            return Result.Failure<PagedResponse<SearchTodoItemResponse>>(
                UserErrors.Unauthorized());
        }

        float[] embedding =
            await embeddingsService.GenerateEmbeddingsForSearchAsync(query.Searchtext);

        PagedResponse<TodoDocument> response =
            await elasticTodoSearchService.SemanticSearchTodosAsync(
                userId: query.UserId,
                embedding: embedding,
                page: query.Pagination?.Page ?? 1,
                size: query.Pagination?.Size ?? 20,
                sorting: query.Sorting,
                candidateLimit: SemanticCandidateLimit,
                cancellationToken);

        List<SearchTodoItemResponse> todos =
        [
            .. response.Items.Select(todo => new SearchTodoItemResponse
            {
                Id = todo.Id,
                UserId = todo.UserId,
                Description = todo.Description,
                DueDate = todo.DueDate,
                IsCompleted = todo.IsCompleted,
                CreatedOn = todo.CreatedOn,
                Priority = (Priority)todo.Priority,

                Labels = todo.Labels,

                Categories = todo.Categories,

                // Elasticsearch returns the score instead of distance.
                Similarity = 0,

                SubItems =
                [
                    .. todo.Subtasks.Select(x => new SearchTodoSubItemResponse
                    {
                        Description = x.Description,
                        CompletedOn = x.CompletedOn,
                        CreatedOn = default,
                        IsCompleted = x.IsCompleted,
                        Order = x.Order,
                        TodoItemId = todo.Id
                    })
                ]
            })
        ];

        return new PagedResponse<SearchTodoItemResponse>(todos, response.Total);
    }
}
