using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.OpenAI.Enrichment;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.EditSubItem;

internal sealed class TodoSubItemsEditedDomainEventHandler() 
    : IDomainEventHandler<TodoSubItemsEditedDomainEvent>
{
    public async Task Handle(TodoSubItemsEditedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        
        // TODO: Send an email verification link, etc.
    }
}
