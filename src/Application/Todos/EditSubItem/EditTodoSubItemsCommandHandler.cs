using Domain;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Data;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Authentication;

namespace Application.Todos.EditSubItem;

internal sealed class EditTodoSubItemsCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<EditTodoSubItemsCommand, Guid>
{
    public async Task<Result<Guid>> Handle(EditTodoSubItemsCommand command, CancellationToken cancellationToken)
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

        TodoItem todoItem = await context.TodoItems.Include(x => x.SubItems).FirstAsync(x => x.Id == command.TodoId, cancellationToken: cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure<Guid>(TodoItemErrors.NotFound(command.TodoId));
        }

        // Existing IDs from request
        HashSet<Guid?> requestIds = [.. command.TodoSubItems.Select(x => x.Id)];

        // 1. Remove deleted items
        var removedItems = todoItem.SubItems.Where(x => !requestIds.Contains(x.Id)).ToList();

        foreach (TodoSubItem item in removedItems)
        {
            todoItem.RemoveSubItem(item.Id);

            context.TodoSubItems.Remove(item);
        }

        // 2. Update existing + add new
        foreach (TodoSubItemCommand requestItem in command.TodoSubItems)
        {
            if (requestItem.Id != Guid.Empty)
            {
                TodoSubItem? existing = todoItem.GetSubItem(requestItem.Id);

                if (existing is not null)
                {
                    existing.Description = requestItem.Description;
                    existing.IsCompleted = requestItem.IsCompleted;
                    existing.Order = requestItem.Order;
                    existing.UpdatedOn = dateTimeProvider.UtcNow;
                }
            }
            else
            {
                todoItem.AddSubItem(
                    new TodoSubItem
                    {
                        Description = requestItem.Description,
                        IsCompleted = requestItem.IsCompleted,
                        Order = requestItem.Order,
                        CreatedOn = dateTimeProvider.UtcNow
                    });
            }
        }

        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        todoItem.Raise(new TodoSubItemsEditedDomainEvent(todoItem.Id));

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.SubtasksUpdated, "Subtask Updated", user.Id));

        await context.SaveChangesAsync(cancellationToken);

        return todoItem.Id;
    }
}
