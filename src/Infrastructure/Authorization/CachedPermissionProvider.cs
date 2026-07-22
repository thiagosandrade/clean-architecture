using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Authorization;

public sealed class CachedPermissionProvider : IPermissionProvider
{
    private readonly IPermissionProvider _permissionProvider;
    private readonly IMemoryCache _cache;

    public CachedPermissionProvider(
        IPermissionProvider permissionProvider,
        IMemoryCache cache)
    {
        _permissionProvider = permissionProvider;
        _cache = cache;
    }

    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        string cacheKey = $"permissions:{userId}";

        if (_cache.TryGetValue(cacheKey, out HashSet<string> cached))
        {
            return cached;
        }

        HashSet<string> permissions = await _permissionProvider.GetForUserIdAsync(userId);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        };

        _cache.Set(cacheKey, permissions, cacheOptions);

        return permissions;
    }
}
