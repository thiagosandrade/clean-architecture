using SharedKernel;

namespace Domain.Todos;

public sealed record TodoSubItemsEditedDomainEvent(Guid TodoItemId) : IDomainEvent;
