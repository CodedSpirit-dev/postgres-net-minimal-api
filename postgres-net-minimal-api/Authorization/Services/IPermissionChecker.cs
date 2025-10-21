using postgres_net_minimal_api.Authorization.Enums;

namespace postgres_net_minimal_api.Authorization.Services;

/// <summary>
/// Service for checking permissions with support for instance-level (ownership) checks
/// </summary>
public interface IPermissionChecker
{
    /// <summary>
    /// Check if user has permission for a resource type
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, ResourceType resource, ActionType action);

    /// <summary>
    /// Check if user has permission AND owns the specific resource instance
    /// This combines type-level permission check with instance-level ownership check
    /// </summary>
    Task<bool> HasPermissionAndOwnsResourceAsync(
        Guid userId,
        ResourceType resource,
        ActionType action,
        Guid resourceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has "Manage" permission for a resource (bypasses ownership check)
    /// Manage permission allows acting on any instance of the resource
    /// </summary>
    Task<bool> CanManageResourceAsync(Guid userId, ResourceType resource);
}
