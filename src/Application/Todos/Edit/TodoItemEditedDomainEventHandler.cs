using Application.Abstractions.Data;
using Application.Abstractions.Extensions;
using Application.OpenAI.Enrichment;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

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

        todoItem.Raise(new TaskActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.CategoriesGenerated, "Categories Generated", todoItem.UserId));

        await context.SaveChangesAsync(cancellationToken);

        // TODO: Send an email verification link, etc.
    }
}
