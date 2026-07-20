using Domain.Permissions;

namespace Domain.Users;

public sealed class UserPermission : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; }
}
