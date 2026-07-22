using System.Linq.Dynamic.Core;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.OpenAI.Embeddings;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Search;

internal sealed class SearchTodosQueryHandler(
    IApplicationDbContext context,
    IEmbeddingsService embeddingsService)
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

        // 1. Generate embedding for search text
        float[] vectorArray = await embeddingsService.GenerateEmbeddingsForSearchAsync(query.Searchtext);

        // 2. Convert to pgvector
        Vector queryVector = vectorArray.ToVector();

        // 3. Semantic search - retrieve closest candidates
        IQueryable<SearchTodoItemResponse> todos =
            context.TodoItems
                .AsNoTracking()
                .Where(x =>
                    x.UserId == query.UserId &&
                    x.Embedding != null)
                .Select(todoItem => new
                {
                    Todo = todoItem,
                    Distance = todoItem.Embedding!.CosineDistance(queryVector)
                })
                .OrderBy(x => x.Distance)
                .Take(SemanticCandidateLimit)
                .Select(x => new SearchTodoItemResponse
                {
                    Id = x.Todo.Id,
                    UserId = x.Todo.UserId,
                    Description = x.Todo.Description,
                    DueDate = x.Todo.DueDate,
                    Labels = x.Todo.Labels,
                    Categories = x.Todo.Categories,
                    IsCompleted = x.Todo.IsCompleted,
                    CreatedAt = x.Todo.CreatedOn,
                    CompletedAt = x.Todo.CompletedAt,
                    Priority = x.Todo.Priority,
                    Similarity = 1 - x.Distance,
                    SubItems = x.Todo.SubItems.Select(y => new SearchTodoSubItemResponse()
                    {
                        CompletedAt = y.CompletedAt,
                        CreatedAt = y.CreatedOn,
                        Order = y.Order,
                        Description = y.Description,
                        IsCompleted = y.IsCompleted,
                        TodoItemId = y.TodoItemId
                    })
                });


        // 4. Count semantic results BEFORE pagination
        int totalItems = await todos.CountAsync(cancellationToken);

        // 5. Apply user sorting AFTER semantic filtering
        if (query.Sorting != null && !string.IsNullOrEmpty(query.Sorting.PropertyName))
        {
            string direction = query.Sorting.Descending
                ? "descending"
                : "ascending";

            todos = todos.OrderBy($"{query.Sorting.PropertyName} {direction}");
        }

        // 6. Apply pagination
        if (query.Pagination is not null)
        {
            todos = todos
                .Skip((query.Pagination.Page - 1) * query.Pagination.Size)
                .Take(query.Pagination.Size);
        }

        // 7. Execute
        List<SearchTodoItemResponse> resultTodos =
            await todos.ToListAsync(cancellationToken);

        return new PagedResponse<SearchTodoItemResponse>(
            resultTodos,
            totalItems);
    }
}
