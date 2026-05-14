using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.Embeddings;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Create;

internal sealed class TodoItemCreatedDomainEventHandler(
    IApplicationDbContext context, 
    IEmbeddingsService embeddingsService,
    IDateTimeProvider dateTimeProvider) 
    : IDomainEventHandler<TodoItemCreatedDomainEvent>
{
    public async Task Handle(TodoItemCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        TodoItem todoItem = await context.TodoItems.FirstAsync(x => x.Id == domainEvent.TodoItemId, cancellationToken: cancellationToken);

        float[] embedding = await embeddingsService.GenerateEmbeddingsAsync(todoItem.Description);

        todoItem.Embedding = embedding.ToVector();
        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        context.TodoItems.Update(todoItem);

        await context.SaveChangesAsync(cancellationToken);

        // TODO: Send an email verification link, etc.
    }
}
