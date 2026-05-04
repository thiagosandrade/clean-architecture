using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(params string[] permissions)
    {
        Permissions = permissions;
    }

    public IEnumerable<string> Permissions { get; }
}
