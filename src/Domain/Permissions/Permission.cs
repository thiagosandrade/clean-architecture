using SharedKernel;

namespace Domain.Permissions;

public sealed class Permission : Entity
{
    public Guid Id { get; set; }
    public string Description { get; set; }
}
