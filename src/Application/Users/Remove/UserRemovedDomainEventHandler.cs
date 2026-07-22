using Domain.DomainEvents;
using Domain.Users;

namespace Application.Users.Remove;

internal sealed class UserRemovedDomainEventHandler : IDomainEventHandler<UserDomainEvents>
{
    public Task Handle(UserDomainEvents domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
