using postgres_net_minimal_api.Authorization.Enums;
using postgres_net_minimal_api.Authorization.Models;

namespace postgres_net_minimal_api.Authorization.Services;

/// <summary>
/// Service for managing and checking user permissions
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Check if a user has a specific permission
    /// </summary>
    Task<bool> UserHasPermissionAsync(Guid userId, ResourceType resource, ActionType action);

    /// <summary>
    /// Get all permissions for a user (cached)
    /// </summary>
    Task<HashSet<string>> GetUserPermissionsAsync(Guid userId);

    /// <summary>
    /// Get all permissions for a role
    /// </summary>
    Task<List<RolePermission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assign a permission to a role
    /// </summary>
    Task<bool> AssignPermissionToRoleAsync(Guid roleId, int featureActionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a permission from a role
    /// </summary>
    Task<bool> RemovePermissionFromRoleAsync(Guid roleId, int featureActionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear cached permissions for a user (call when role permissions change)
    /// </summary>
    void ClearUserPermissionCache(Guid userId);

    /// <summary>
    /// Clear all permission caches for a role (call when role permissions change)
    /// </summary>
    Task ClearRolePermissionCacheAsync(Guid roleId, CancellationToken cancellationToken = default);
}
