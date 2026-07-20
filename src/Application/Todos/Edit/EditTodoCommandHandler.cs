using Domain;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Data;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Authentication;

namespace Application.Todos.Edit;

internal sealed class EditTodoCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<EditTodoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(EditTodoCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized());
        }

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        TodoItem todoItem = await context.TodoItems.FirstAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure<Guid>(TodoItemErrors.NotFound(command.Id));
        }

        bool needsEnrichment = false;
        
        if (todoItem.Description != command.Description && todoItem.Labels != command.Labels)
        {
            needsEnrichment = true;
        }

        todoItem.Description = command.Description;
        todoItem.Priority = command.Priority;
        todoItem.DueDate = command.DueDate;
        todoItem.Labels = command.Labels;
        todoItem.UserId = command.UserId;
        todoItem.UpdatedOn = dateTimeProvider.UtcNow;


        todoItem.IsCompleted = command.IsCompleted;
        todoItem.CompletedAt = command.IsCompleted ? dateTimeProvider.UtcNow : null;


        context.TodoItems.Update(todoItem);

        if (needsEnrichment)
        {
            todoItem.Raise(new TodoItemEditedDomainEvent(todoItem.Id));
        }

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.TaskUpdated, "Task Updated", user.Id));

        await context.SaveChangesAsync(cancellationToken);

        return todoItem.Id;
    }
}
