using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.GetById;

internal sealed class GetTodoByIdQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTodoByIdQuery, TodoResponse>
{
    public async Task<Result<TodoResponse>> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken)
    {
        TodoResponse? todo = await context.TodoItems
            .Include(x => x.SubItems)
            .Include(x => x.Dependencies)
            .AsNoTracking()
            .Where(todoItem => todoItem.Id == query.TodoItemId && todoItem.UserId == userContext.UserId)
            .Select(todoItem => new TodoResponse
            {
                Id = todoItem.Id,
                UserId = todoItem.UserId,
                Description = todoItem.Description,
                DueDate = todoItem.DueDate,
                Labels = todoItem.Labels,
                IsCompleted = todoItem.IsCompleted,
                Priority = todoItem.Priority,
                CreatedAt = todoItem.CreatedOn,
                CompletedAt = todoItem.CompletedAt,
                Categories = todoItem.Categories,
                Subtasks = todoItem.SubItems.OrderBy(x => x.Order).Select(x => new TodoSubItemResponse()
                {
                    Id  = x.Id,
                    TodoItemId = x.TodoItemId,
                    Description = x.Description,
                    CreatedAt = x.CreatedOn,
                    CompletedAt = x.CompletedAt,
                    IsCompleted = x.IsCompleted,
                    Order = x.Order
                }),
                Dependencies = todoItem.Dependencies.Select(x => new TaskDependencyResponse()
                {
                    TodoItemId = x.TodoItemId,
                    DependsOnTodoItemId = x.DependsOnTodoItemId,
                    Description = x.DependsOnTodoItem.Description
                })
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (todo is null)
        {
            return Result.Failure<TodoResponse>(TodoItemErrors.NotFound(query.TodoItemId));
        }

        return todo;
    }
}
