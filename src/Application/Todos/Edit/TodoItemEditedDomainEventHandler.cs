using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.Embeddings;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Edit;

internal sealed class TodoItemEditedDomainEventHandler(
    IApplicationDbContext context, 
    IEmbeddingsService embeddingsService,
    IDateTimeProvider dateTimeProvider) 
    : IDomainEventHandler<TodoItemEditedDomainEvent>
{
    public async Task Handle(TodoItemEditedDomainEvent domainEvent, CancellationToken cancellationToken)
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
