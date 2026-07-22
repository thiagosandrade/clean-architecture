using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Users.Remove;

internal sealed class RemoveUserCommandHandler(IRabbitMqPublisher publisher, IApplicationDbContext context)
    : ICommandHandler<RemoveUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        if (!await CheckIfUserExists(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        User user = await LoadUser(command, cancellationToken);

        user.Raise(new UserRemovedDomainEvent(user.Id));

        context.Users.Remove(user);

        await context.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(new UserDeletedIntegrationEvent(user.Id), cancellationToken);

        return user.Id;
    }

    private async Task<User> LoadUser(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        return await context.Users.FirstAsync(x => x.Id == command.UserId, cancellationToken);
    }

    private async Task<bool> CheckIfUserExists(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
    }
}
