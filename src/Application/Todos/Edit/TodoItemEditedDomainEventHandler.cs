using Application.Common.Interfaces;
using Application.OpenAI.Enrichment;
using Domain.Activities;
using Domain.DomainEvents;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;

namespace Application.Todos.Edit;

internal sealed class TodoItemEditedDomainEventHandler(
    IApplicationDbContext context,
    ICategoryEnrichmentService categoryEnrichmentService,
    IDateTimeProvider dateTimeProvider) 
    : IDomainEventHandler<TodoItemEditedDomainEvent>
{
    public async Task Handle(TodoItemEditedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        TodoItem todoItem = await context.TodoItems.FirstAsync(x => x.Id == domainEvent.TodoItemId, cancellationToken: cancellationToken);

        IReadOnlyCollection<string> categories = await categoryEnrichmentService.EnrichAsync(todoItem.Description, todoItem.Labels, cancellationToken);

        todoItem.Categories = [.. categories];

        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        context.TodoItems.Update(todoItem);

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.CategoriesGenerated, "Categories Generated", todoItem.UserId));

        await context.SaveChangesAsync(cancellationToken);

        // TODO: Send an email verification link, etc.
    }
}
