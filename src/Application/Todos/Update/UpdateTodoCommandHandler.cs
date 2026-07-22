using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.Activities;
using Domain.API;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Update;

internal sealed class UpdateTodoCommandHandler(
    IRabbitMqPublisher publisher,
    IDateTimeProvider dateTimeProvider,
    IApplicationDbContext context)
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
        
        await publisher.PublishAsync(new TodoUpdatedIntegrationEvent(todoItem.Id, todoItem.UserId, todoItem.Description), cancellationToken);

        return Result.Success();
    }
}
