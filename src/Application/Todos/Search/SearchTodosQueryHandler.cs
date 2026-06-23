using System.Linq.Dynamic.Core;
using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.Abstractions.Messaging;
using Application.Embeddings;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Search;

internal sealed class SearchTodosQueryHandler(
    IApplicationDbContext context,
    IEmbeddingsService embeddingsService)
    : IQueryHandler<SearchTodosQuery, PagedResponse<SearchTodoResponse>>
{
    private const int SemanticCandidateLimit = 1000;

    public async Task<Result<PagedResponse<SearchTodoResponse>>> Handle(
        SearchTodosQuery query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Searchtext))
        {
            return Result.Failure<PagedResponse<SearchTodoResponse>>(
                UserErrors.Unauthorized());
        }

        // 1. Generate embedding for search text
        float[] vectorArray = await embeddingsService.GenerateEmbeddingsAsync(query.Searchtext);

        // 2. Convert to pgvector
        Vector queryVector = vectorArray.ToVector();

        // 3. Semantic search - retrieve closest candidates
        IQueryable<SearchTodoResponse> todos =
            context.TodoItems
                .AsNoTracking()
                .Where(x =>
                    x.UserId == query.UserId &&
                    x.Embedding != null)
                .OrderBy(x =>
                    x.Embedding!.CosineDistance(queryVector))
                .Take(SemanticCandidateLimit)
                .Select(todoItem => new SearchTodoResponse
                {
                    Id = todoItem.Id,
                    UserId = todoItem.UserId,
                    Description = todoItem.Description,
                    DueDate = todoItem.DueDate,
                    Labels = todoItem.Labels,
                    IsCompleted = todoItem.IsCompleted,
                    CreatedAt = todoItem.CreatedAt,
                    CompletedAt = todoItem.CompletedAt,
                    Priority = todoItem.Priority
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
        List<SearchTodoResponse> resultTodos =
            await todos.ToListAsync(cancellationToken);

        return new PagedResponse<SearchTodoResponse>(
            resultTodos,
            totalItems);
    }
}
