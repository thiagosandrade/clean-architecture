using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.Activities;
using Domain.API;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Create;

internal sealed class CreateTodoCommandHandler(
    IApplicationDbContext context,
    IRabbitMqPublisher publisher,
    IUserContext userContext)
    : ICommandHandler<CreateTodoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
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

        var todoItem = new TodoItem
        {
            UserId = user.Id,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Labels = command.Labels,
            IsCompleted = false
        };

        context.TodoItems.Add(todoItem);

        await context.SaveChangesAsync(cancellationToken);

        todoItem.Raise(new TodoItemCreatedDomainEvent(todoItem.Id));
        
        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.TaskCreated, "Task Created", user.Id));

        await publisher.PublishAsync(new TodoCreatedIntegrationEvent(todoItem.Id, todoItem.UserId, todoItem.Description), cancellationToken);

        return todoItem.Id;
    }
}
