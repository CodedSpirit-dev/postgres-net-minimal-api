using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using postgres_net_minimal_api.Authorization.Enums;
using postgres_net_minimal_api.Authorization.Models;
using postgres_net_minimal_api.Data;

namespace postgres_net_minimal_api.Authorization.Services;

/// <summary>
/// Service for managing and checking user permissions with memory caching
/// </summary>
public class PermissionService(AppDbContext context, IMemoryCache cache) : IPermissionService
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public async Task<bool> UserHasPermissionAsync(Guid userId, ResourceType resource, ActionType action)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        var permissionKey = $"{resource}.{action}";
        return permissions.Contains(permissionKey);
    }

    public async Task<HashSet<string>> GetUserPermissionsAsync(Guid userId)
    {
        var cacheKey = $"UserPermissions_{userId}";

        if (cache.TryGetValue<HashSet<string>>(cacheKey, out var cachedPermissions))
        {
            return cachedPermissions!;
        }

        // Get user's role
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.RoleId })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return [];
        }

        // Get all permissions for the user's role
        var permissions = await context.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == user.RoleId)
            .Include(rp => rp.FeatureAction)
                .ThenInclude(fa => fa.Feature)
            .Include(rp => rp.FeatureAction)
                .ThenInclude(fa => fa.Action)
            .Where(rp => rp.FeatureAction.IsEnabled)
            .Select(rp => $"{rp.FeatureAction.Feature.ResourceKey}.{rp.FeatureAction.Action.ActionKey}")
            .ToListAsync();

        var permissionSet = permissions.ToHashSet();

        // Cache the permissions
        cache.Set(cacheKey, permissionSet, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheExpiration,
            SlidingExpiration = TimeSpan.FromMinutes(10)
        });

        return permissionSet;
    }

    public async Task<List<RolePermission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await context.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.FeatureAction)
                .ThenInclude(fa => fa.Feature)
            .Include(rp => rp.FeatureAction)
                .ThenInclude(fa => fa.Action)
            .OrderBy(rp => rp.FeatureAction.Feature.DisplayOrder)
            .ThenBy(rp => rp.FeatureAction.Action.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AssignPermissionToRoleAsync(Guid roleId, int featureActionId, CancellationToken cancellationToken = default)
    {
        // Check if permission already exists
        var exists = await context.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.FeatureActionId == featureActionId, cancellationToken);

        if (exists)
        {
            return false; // Already assigned
        }

        // Verify the role and feature action exist
        var roleExists = await context.UserRoles.AnyAsync(r => r.Id == roleId, cancellationToken);
        var featureActionExists = await context.FeatureActions.AnyAsync(fa => fa.Id == featureActionId, cancellationToken);

        if (!roleExists || !featureActionExists)
        {
            return false;
        }

        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            FeatureActionId = featureActionId,
            CreatedAt = DateTime.UtcNow
        };

        context.RolePermissions.Add(rolePermission);
        await context.SaveChangesAsync(cancellationToken);

        // Clear cache for all users with this role
        await ClearRolePermissionCacheAsync(roleId, cancellationToken);

        return true;
    }

    public async Task<bool> RemovePermissionFromRoleAsync(Guid roleId, int featureActionId, CancellationToken cancellationToken = default)
    {
        var rolePermission = await context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.FeatureActionId == featureActionId, cancellationToken);

        if (rolePermission == null)
        {
            return false;
        }

        context.RolePermissions.Remove(rolePermission);
        await context.SaveChangesAsync(cancellationToken);

        // Clear cache for all users with this role
        await ClearRolePermissionCacheAsync(roleId, cancellationToken);

        return true;
    }

    public void ClearUserPermissionCache(Guid userId)
    {
        var cacheKey = $"UserPermissions_{userId}";
        cache.Remove(cacheKey);
    }

    public async Task ClearRolePermissionCacheAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        // Get all users with this role
        var userIds = await context.Users
            .AsNoTracking()
            .Where(u => u.RoleId == roleId)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        // Clear cache for each user
        foreach (var userId in userIds)
        {
            ClearUserPermissionCache(userId);
        }
    }
}
