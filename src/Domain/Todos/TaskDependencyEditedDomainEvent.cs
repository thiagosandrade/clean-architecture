using SharedKernel;

namespace Domain.Todos;

public sealed record TaskDependencyEditedDomainEvent(Guid TodoItemId) : IDomainEvent;
