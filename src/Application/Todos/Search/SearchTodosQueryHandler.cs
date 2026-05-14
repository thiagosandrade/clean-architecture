using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.Abstractions.Messaging;
using Application.Embeddings;
using Application.Todos.Get;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Search;

internal sealed class SearchTodosQueryHandler(IApplicationDbContext context, IEmbeddingsService embeddingsService)
    : IQueryHandler<SearchTodosQuery, List<SearchTodoResponse>>
{
    public async Task<Result<List<SearchTodoResponse>>> Handle(SearchTodosQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(query.searchtext))
        {
            return Result.Failure<List<SearchTodoResponse>>(UserErrors.Unauthorized());
        }

        // 1. Generate embedding
        float[] vectorArray = await embeddingsService.GenerateEmbeddingsAsync(query.searchtext);

        // 2. Convert to pgvector
        Vector queryVector = vectorArray.ToVector();

        List<SearchTodoResponse> todos = await context.TodoItems
            .AsNoTracking()
            .Where(x => x.Embedding != null)
            .OrderBy(x => x.Embedding!.CosineDistance(queryVector))
            .Select(todoItem => new SearchTodoResponse
            {
                Id = todoItem.Id,
                UserId = todoItem.UserId,
                Description = todoItem.Description,
                DueDate = todoItem.DueDate,
                Labels = todoItem.Labels,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedAt,
                CompletedAt = todoItem.CompletedAt
            })
            .ToListAsync(cancellationToken);

        return todos;
    }
}
