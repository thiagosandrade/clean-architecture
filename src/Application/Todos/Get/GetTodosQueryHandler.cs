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
                Categories = todoItem.Categories,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedOn,
                CompletedAt = todoItem.CompletedAt,
                Priority = todoItem.Priority,
                Subtasks = todoItem.SubItems.Select(y => new TodoSubtaskResponse()
                {
                    TodoItemId = y.TodoItemId,
                    Id = y.Id,
                    Description = y.Description,
                    IsCompleted = y.IsCompleted,
                    Order = y.Order,
                    CreatedAt = y.CreatedOn,
                    CompletedAt = y.CompletedAt
                })
            });

        //QUERYING
        if (query.Filter is not null)
        {
            if (query.Filter.Priority.HasValue)
            {
                todos = todos.Where(x => x.Priority == query.Filter.Priority.Value);
            }

            if (query.Filter.IsCompleted.HasValue)
            {
                todos = todos.Where(x => x.IsCompleted == query.Filter.IsCompleted.Value);
            }

            if (query.Filter.DueDateFrom.HasValue && query.Filter.DueDateTo.HasValue)
            {
                var start = query.Filter.DueDateFrom.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                var end = query.Filter.DueDateTo.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                todos = todos.Where(x => x.DueDate >= start && x.DueDate < end);
            }

            if (query.Filter.DueDateFrom.HasValue && !query.Filter.DueDateTo.HasValue)
            {
                var start = query.Filter.DueDateFrom.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                todos = todos.Where(x => x.DueDate >= start);
            }

            if (!query.Filter.DueDateFrom.HasValue && query.Filter.DueDateTo.HasValue)
            {
                var end = query.Filter.DueDateTo.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                todos = todos.Where(x => x.DueDate < end);
            }
        }

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
