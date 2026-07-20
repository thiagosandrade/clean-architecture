namespace Domain.Activities;

public sealed record TodoActivityLogRequestedDomainEvent(Guid TodoItemId, TaskActivityType TaskActivityType, string Description, Guid UserId) : IDomainEvent;
