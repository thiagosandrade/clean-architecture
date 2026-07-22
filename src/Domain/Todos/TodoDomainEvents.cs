using Domain.DomainEvents;

namespace Domain.Todos;

public sealed record TodoDomainEvents(Guid TodoItemId) : IDomainEvent;

public sealed record TodoItemCreatedDomainEvent(Guid TodoItemId) : IDomainEvent;
public sealed record TodoItemDeletedDomainEvent(Guid TodoItemId) : IDomainEvent;
public sealed record TodoItemEditedDomainEvent(Guid TodoItemId) : IDomainEvent;

public sealed record TodoSubItemsEditedDomainEvent(Guid TodoItemId) : IDomainEvent;
public sealed record TaskDependencyEditedDomainEvent(Guid TodoItemId) : IDomainEvent;
