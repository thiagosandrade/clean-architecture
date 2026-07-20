using Microsoft.AspNetCore.Authorization;

namespace SharedKernel.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(params string[] permissions)
        : base(policy: string.Join(",", permissions.Select(p => p.ToString())))
    {
    }
}
