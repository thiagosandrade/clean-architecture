using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Update;

internal sealed class UpdateTodoCommandHandler(
    IApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateTodoCommand>
{
    public async Task<Result> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        TodoItem? todoItem = await context.TodoItems
            .SingleOrDefaultAsync(t => t.Id == command.TodoItemId, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        }

        todoItem.Description = command.Description;
        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.TaskUpdated, "Subtask Updated", todoItem.UserId));

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
