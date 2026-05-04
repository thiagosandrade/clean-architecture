using SharedKernel;

namespace Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
public sealed record UserUpdatedDomainEvent(Guid UserId) : IDomainEvent;
public sealed record UserRemovedDomainEvent(Guid UserId) : IDomainEvent;
public sealed record SetPermissionToUserDomainEvent(Guid UserId, Guid PermissionId) : IDomainEvent;
public sealed record RemovePermissionFromUserDomainEvent(Guid UserId, Guid PermissionId) : IDomainEvent;
