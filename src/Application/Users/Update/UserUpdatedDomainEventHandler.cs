using Domain;
using Domain.Users;

namespace Application.Users.Update;

internal sealed class UserUpdatedDomainEventHandler : IDomainEventHandler<UserUpdatedDomainEvent>
{
    public Task Handle(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
