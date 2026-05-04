using Domain.Users;
using SharedKernel;

namespace Application.Users.Register;

internal sealed class RemovePermissionToUserDomainEventHandler : IDomainEventHandler<SetPermissionToUserDomainEvent>
{
    public Task Handle(SetPermissionToUserDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
