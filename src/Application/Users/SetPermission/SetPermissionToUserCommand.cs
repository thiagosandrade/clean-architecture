using SharedKernel.Abstractions.Messaging;

namespace Application.Users.SetPermission;

public sealed record SetPermissionToUserCommand(Guid PermissionId, Guid UserId)
    : ICommand<Guid>;
