using Domain;
using Domain.Todos;

namespace Application.Todos.EditSubItem;

internal sealed class TodoSubItemsEditedDomainEventHandler() 
    : IDomainEventHandler<TodoSubItemsEditedDomainEvent>
{
    public async Task Handle(TodoSubItemsEditedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        
        // TODO: Send an email verification link, etc.
    }
}
