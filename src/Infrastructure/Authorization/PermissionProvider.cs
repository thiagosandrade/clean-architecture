using Application.Abstractions.Data;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

public interface IPermissionProvider
{
    Task<HashSet<string>> GetForUserIdAsync(Guid userId);
}

public class PermissionProvider(IApplicationDbContext context) : IPermissionProvider
{
    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        List<string> userPermissions = await context
            .UserPermissions
            .Include(x => x.Permission)
            .Where(x => x.UserId == userId).Select(x => x.Permission.Description).ToListAsync();

        HashSet<string> permissionsSet = [.. userPermissions];

        return permissionsSet;
    }
}
