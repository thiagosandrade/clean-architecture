using Microsoft.AspNetCore.Authorization;

namespace SharedKernel.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(params string[] permissions)
    {
        Permissions = permissions;
    }

    public IEnumerable<string> Permissions { get; }
}
