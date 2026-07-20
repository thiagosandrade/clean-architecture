using SharedKernel.Abstractions.Messaging;

namespace Application.Users.RemovePermission;

public sealed record RemovePermissionFromUserCommand(Guid PermissionId, Guid UserId)
    : ICommand<Guid>;
