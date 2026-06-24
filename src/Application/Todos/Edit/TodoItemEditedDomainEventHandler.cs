using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.OpenAI.Enrichment;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Edit;

internal sealed class TodoItemEditedDomainEventHandler(
    IApplicationDbContext context,
    IEnrichmentService enrichmentService,
    IDateTimeProvider dateTimeProvider) 
    : IDomainEventHandler<TodoItemEditedDomainEvent>
{
    public async Task Handle(TodoItemEditedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        TodoItem todoItem = await context.TodoItems.FirstAsync(x => x.Id == domainEvent.TodoItemId, cancellationToken: cancellationToken);

        EnrichmentResult enrichmentResult = await enrichmentService.EnrichAsync(todoItem.Description, todoItem.Labels, cancellationToken);
        
        todoItem.Embedding = enrichmentResult.Embedding.ToVector();
        todoItem.Categories = [.. enrichmentResult.Categories];
        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        context.TodoItems.Update(todoItem);

        await context.SaveChangesAsync(cancellationToken);

        // TODO: Send an email verification link, etc.
    }
}
