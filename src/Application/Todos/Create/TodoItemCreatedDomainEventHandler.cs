using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.OpenAI.Embeddings;
using Application.OpenAI.Enrichment;
using Domain.Activities;
using Domain.DomainEvents;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;

namespace Application.Todos.Create;

internal sealed class TodoItemCreatedDomainEventHandler() 
    : IDomainEventHandler<TodoItemCreatedDomainEvent>
{
    public async Task Handle(TodoItemCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        
    }
}
