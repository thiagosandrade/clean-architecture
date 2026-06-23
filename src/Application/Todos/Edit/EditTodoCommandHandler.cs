using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

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

        todoItem.Description = command.Description;
        todoItem.Priority = command.Priority;
        todoItem.DueDate = command.DueDate;
        todoItem.Labels = command.Labels;
        todoItem.UserId = command.UserId;
        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        if(!todoItem.IsCompleted && command.IsCompleted)
        {
            todoItem.IsCompleted = command.IsCompleted;
            todoItem.CompletedAt = dateTimeProvider.UtcNow;
        }

        context.TodoItems.Update(todoItem);

        todoItem.Raise(new TodoItemEditedDomainEvent(todoItem.Id));

        await context.SaveChangesAsync(cancellationToken);
        
        return todoItem.Id;
    }
}
