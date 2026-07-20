using Domain;
using Domain.Users;

namespace Application.Users.RemovePermission;

internal sealed class RemovePermissionFromUserDomainEventHandler : IDomainEventHandler<RemovePermissionFromUserDomainEvent>
{
    public Task Handle(RemovePermissionFromUserDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
