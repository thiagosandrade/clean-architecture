using Domain.DomainEvents;
using Domain.Users;

namespace Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserDomainEvents>
{
    public Task Handle(UserDomainEvents domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
