using Domain.Users;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User is { Identity.IsAuthenticated: false })
        {
            context.Fail();

            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IPermissionProvider permissionProvider = scope.ServiceProvider.GetRequiredService<IPermissionProvider>();

        Guid userId = context.User.GetUserId();

        HashSet<string> userPermissions = await permissionProvider.GetForUserIdAsync(userId);

        if (requirement.Permissions.Any(permission => userPermissions.Contains(permission)))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
