using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Users.RemovePermission;

internal sealed class RemovePermissionFromUserCommandHandler(IRabbitMqPublisher publisher, IApplicationDbContext context)
    : ICommandHandler<RemovePermissionFromUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RemovePermissionFromUserCommand command, CancellationToken cancellationToken)
    {
        if (await CheckIfUserExists(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        if (!await CheckIfPermissionExists(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.PermissionNotFound(command.PermissionId));
        }

        if (!await CheckIfPermissionExistsForUser(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.PermissionAlreadyExistsForUser(command.UserId, command.PermissionId));
        }

        UserPermission userPermission = await context.UserPermissions.FirstAsync(x => x.UserId == command.UserId &&  x.PermissionId == command.PermissionId, cancellationToken);

        context.UserPermissions.Remove(userPermission);

        await context.SaveChangesAsync(cancellationToken);

        userPermission.Raise(new RemovePermissionFromUserDomainEvent(userPermission.UserId, userPermission.Id));

        await publisher.PublishAsync(new UserUpdatedIntegrationEvent(command.UserId), cancellationToken);

        return userPermission.Id;
    }

    private async Task<bool> CheckIfPermissionExistsForUser(RemovePermissionFromUserCommand command, CancellationToken cancellationToken)
    {
        return await context.UserPermissions.AnyAsync(x => x.UserId == command.UserId && x.PermissionId == command.PermissionId, cancellationToken);
    }

    private async Task<bool> CheckIfPermissionExists(RemovePermissionFromUserCommand command, CancellationToken cancellationToken)
    {
        bool permissionExists = await context.Permissions.AnyAsync(x => x.Id == command.PermissionId, cancellationToken);

        return permissionExists;
    }

    private async Task<bool> CheckIfUserExists(RemovePermissionFromUserCommand command, CancellationToken cancellationToken)
    {
        return !await context.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
    }
}
