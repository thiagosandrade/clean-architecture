using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.OpenAI;
using Application.OpenAI.Embeddings;
using Application.OpenAI.Enrichment;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

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
