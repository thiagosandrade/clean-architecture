using Domain.Activities;
using Domain.DomainEvents;

namespace Application.Todos.Activities.Log;

internal sealed class TodoActivityLogRequestedDomainEventHandler(ITodoActivityService todoActivityService) : IDomainEventHandler<TodoActivityLogRequestedDomainEvent>
{
    public async Task Handle(TodoActivityLogRequestedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await todoActivityService.LogAsync(
            domainEvent.TodoItemId,
            domainEvent.TaskActivityType,
            domainEvent.Description,
            domainEvent.UserId,
            cancellationToken: cancellationToken);
    }
}
