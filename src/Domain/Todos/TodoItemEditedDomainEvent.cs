using SharedKernel;

namespace Domain.Todos;

public sealed record TodoItemEditedDomainEvent(Guid TodoItemId) : IDomainEvent;
