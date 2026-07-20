using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Todos.Dependency;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.EditSubItem;

internal sealed class EditTodoDependenciesCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<EditTodoDependenciesCommand, Guid>
{
    public async Task<Result<Guid>> Handle(EditTodoDependenciesCommand command, CancellationToken cancellationToken)
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

        TodoItem todoItem = await context.TodoItems.Include(x => x.Dependencies).FirstAsync(x => x.Id == command.TodoId, cancellationToken: cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure<Guid>(TodoItemErrors.NotFound(command.TodoId));
        }

        if (command.Dependencies.Any(x => x == command.TodoId))
        {
            return Result.Failure<Guid>(DependencyErrors.CannotDependOnItself(command.TodoId));
        }

        foreach (Guid item in command.Dependencies)
        {
            TodoItem? dependencyTask = await context.TodoItems.Where(x => x.Id == item).FirstOrDefaultAsync(cancellationToken);

            if(dependencyTask is null)
            {
                return Result.Failure<Guid>(DependencyErrors.DependencyNotFound(command.TodoId, item));
            }

            if (command.UserId != dependencyTask.UserId)
            {
                return Result.Failure<Guid>(DependencyErrors.CannotDependOnAnotherUserTask(command.TodoId, item));
            }
        }

        // Hashset IDs from request
        var incoming = command.Dependencies.ToHashSet();

        // Existing IDs
        var existing = todoItem.Dependencies
            .Select(x => x.DependsOnTodoItemId)
            .ToHashSet();

        // 1. Remove deleted items
        foreach (TodoDependency? dependency in todoItem.Dependencies
             .Where(x => !incoming.Contains(x.DependsOnTodoItemId))
             .ToList())
        {
            context.TodoDependencies.Remove(dependency);
        }

        // 2. Insert new
        foreach (Guid id in incoming.Except(existing))
        {
            todoItem.AddDependency(id);
        }

        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        todoItem.Raise(new TaskDependencyEditedDomainEvent(todoItem.Id));

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.DependencyUpdated, "Dependency Updated", user.Id));

        await context.SaveChangesAsync(cancellationToken);

        return todoItem.Id;
    }
}
