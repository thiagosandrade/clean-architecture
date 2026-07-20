using Domain;
using Domain.Users;

namespace Application.Users.SetPermission;

internal sealed class RemovePermissionToUserDomainEventHandler : IDomainEventHandler<SetPermissionToUserDomainEvent>
{
    public Task Handle(SetPermissionToUserDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
