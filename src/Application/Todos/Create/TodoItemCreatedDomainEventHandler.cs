using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.OpenAI;
using Application.OpenAI.Embeddings;
using Application.OpenAI.Enrichment;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Create;

internal sealed class TodoItemCreatedDomainEventHandler(
    IApplicationDbContext context, 
    ICategoryEnrichmentService categoryEnrichmentService,
    IEmbeddingsService embeddingsService,
    IDateTimeProvider dateTimeProvider) 
    : IDomainEventHandler<TodoItemCreatedDomainEvent>
{
    public async Task Handle(TodoItemCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        TodoItem todoItem = await context.TodoItems.FirstAsync(x => x.Id == domainEvent.TodoItemId, cancellationToken: cancellationToken);

        IReadOnlyCollection<string> categories = await categoryEnrichmentService.EnrichAsync(todoItem.Description, todoItem.Labels, cancellationToken);

        float[] embedding = await embeddingsService.GenerateEmbeddingsAsync(todoItem.Description, [.. todoItem.Labels], categories);

        todoItem.Embedding = embedding.ToVector();
        todoItem.Categories = [.. categories];

        todoItem.UpdatedOn = dateTimeProvider.UtcNow;
        
        context.TodoItems.Update(todoItem);

        await context.SaveChangesAsync(cancellationToken);

        // TODO: Send an email verification link, etc.
    }
}
