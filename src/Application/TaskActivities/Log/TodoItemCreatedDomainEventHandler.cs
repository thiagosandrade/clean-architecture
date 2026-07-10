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

namespace Application.TaskActivities.Log;

internal sealed class TaskActivityLogRequestedDomainEventHandler(ITaskActivityService taskActivityService) : IDomainEventHandler<TaskActivityLogRequestedDomainEvent>
{
    public async Task Handle(TaskActivityLogRequestedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await taskActivityService.LogAsync(
            domainEvent.TodoItemId,
            domainEvent.TaskActivityType,
            domainEvent.Description,
            domainEvent.UserId,
            cancellationToken: cancellationToken);
    }
}
