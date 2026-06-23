using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System.Linq.Dynamic.Core;

namespace Application.Todos.Get;

internal sealed class GetTodosQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTodosQuery, PagedResponse<TodoResponse>>
{
    public async Task<Result<PagedResponse<TodoResponse>>> Handle(GetTodosQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<PagedResponse<TodoResponse>>(UserErrors.Unauthorized());
        }

        IQueryable<TodoResponse> todos = context.TodoItems
            .AsNoTracking()
            .Where(todoItem => todoItem.UserId == query.UserId)
            .Select(todoItem => new TodoResponse
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

        int totalItems = await todos.CountAsync(cancellationToken);

        // SORTING
        if (query.Sorting is not null)
        {
            string direction = query.Sorting.Descending
                ? "descending"
                : "ascending";

            todos = todos.OrderBy(
                $"{query.Sorting.PropertyName} {direction}");
        }


        // PAGINATION
        if (query.Pagination is not null)
        {
            todos = todos
                .Skip((query.Pagination.Page - 1) * query.Pagination.Size)
                .Take(query.Pagination.Size);
        }

        List<TodoResponse> resultTodos = await todos.ToListAsync(cancellationToken);

        var result = new PagedResponse<TodoResponse>(resultTodos, totalItems);

        return result;
    }
}
