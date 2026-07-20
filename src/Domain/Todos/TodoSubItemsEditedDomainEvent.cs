namespace Domain.Todos;

public sealed record TodoSubItemsEditedDomainEvent(Guid TodoItemId) : IDomainEvent;
