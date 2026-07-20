using Domain;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Data;
using SharedKernel.Abstractions.Messaging;

namespace Application.Users.SetPermission;

internal sealed class SetPermissionToUserCommandHandler(IApplicationDbContext context)
    : ICommandHandler<SetPermissionToUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(SetPermissionToUserCommand command, CancellationToken cancellationToken)
    {
        if (await CheckIfUserExists(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        if (!await CheckIfPermissionExists(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.PermissionNotFound(command.PermissionId));
        }

        if (await CheckIfPermissionExistsForUser(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.PermissionAlreadyExistsForUser(command.UserId, command.PermissionId));
        }

        var permission = new UserPermission
        {
            UserId = command.UserId,
            PermissionId = command.PermissionId
        };

        context.UserPermissions.Add(permission);

        await context.SaveChangesAsync(cancellationToken);

        permission.Raise(new SetPermissionToUserDomainEvent(permission.UserId, permission.Id));

        return permission.Id;
    }

    private async Task<bool> CheckIfPermissionExistsForUser(SetPermissionToUserCommand command, CancellationToken cancellationToken)
    {
        return await context.UserPermissions.AnyAsync(x => x.UserId == command.UserId && x.PermissionId == command.PermissionId, cancellationToken);
    }

    private async Task<bool> CheckIfPermissionExists(SetPermissionToUserCommand command, CancellationToken cancellationToken)
    {
        bool permissionExists = await context.Permissions.AnyAsync(x => x.Id == command.PermissionId, cancellationToken);

        return permissionExists;
    }

    private async Task<bool> CheckIfUserExists(SetPermissionToUserCommand command, CancellationToken cancellationToken)
    {
        return !await context.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
    }
}
