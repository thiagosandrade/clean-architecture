using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.API;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Delete;

internal sealed class DeleteTodoCommandHandler(IApplicationDbContext context, IRabbitMqPublisher publisher, IUserContext userContext)
    : ICommandHandler<DeleteTodoCommand>
{
    public async Task<Result> Handle(DeleteTodoCommand command, CancellationToken cancellationToken)
    {
        TodoItem? todoItem = await context.TodoItems
            .SingleOrDefaultAsync(t => t.Id == command.TodoItemId && t.UserId == userContext.UserId, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        }

        context.TodoItems.Remove(todoItem);

        todoItem.Raise(new TodoItemDeletedDomainEvent(todoItem.Id));

        await context.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(new TodoDeletedIntegrationEvent(todoItem.Id, todoItem.UserId, todoItem.Description), cancellationToken);

        return Result.Success();
    }
}
