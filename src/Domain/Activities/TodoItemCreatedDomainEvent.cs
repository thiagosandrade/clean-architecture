using SharedKernel;

namespace Domain.Activities;

public sealed record TaskActivityLogRequestedDomainEvent(Guid TodoItemId, TaskActivityType TaskActivityType, string Description, Guid UserId) : IDomainEvent;
