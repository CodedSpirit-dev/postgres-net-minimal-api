using postgres_net_minimal_api.Authorization.Enums;

namespace postgres_net_minimal_api.Authorization.Services;

/// <summary>
/// Implementation of permission checking with ownership validation
/// </summary>
public class PermissionChecker(
    IPermissionService permissionService,
    IServiceProvider serviceProvider) : IPermissionChecker
{
    public async Task<bool> HasPermissionAsync(Guid userId, ResourceType resource, ActionType action)
    {
        return await permissionService.UserHasPermissionAsync(userId, resource, action);
    }

    public async Task<bool> HasPermissionAndOwnsResourceAsync(
        Guid userId,
        ResourceType resource,
        ActionType action,
        Guid resourceId,
        CancellationToken cancellationToken = default)
    {
        // First check if user has the type-level permission
        var hasPermission = await permissionService.UserHasPermissionAsync(userId, resource, action);
        if (!hasPermission)
        {
            return false;
        }

        // Check if user has "Manage" permission, which bypasses ownership check
        var canManage = await permissionService.UserHasPermissionAsync(userId, resource, ActionType.Manage);
        if (canManage)
        {
            return true; // Manage permission allows acting on any instance
        }

        // Check ownership using the appropriate checker
        var ownershipChecker = GetOwnershipChecker(resource);
        if (ownershipChecker == null)
        {
            // If no ownership checker is defined, only rely on permission
            return true;
        }

        return await ownershipChecker.UserOwnsResourceAsync(userId, resourceId, cancellationToken);
    }

    public async Task<bool> CanManageResourceAsync(Guid userId, ResourceType resource)
    {
        return await permissionService.UserHasPermissionAsync(userId, resource, ActionType.Manage);
    }

    private IResourceOwnershipChecker? GetOwnershipChecker(ResourceType resource)
    {
        // Get the appropriate ownership checker based on resource type
        return resource switch
        {
            ResourceType.Posts => serviceProvider.GetService<PostOwnershipChecker>(),
            ResourceType.Comments => serviceProvider.GetService<CommentOwnershipChecker>(),
            ResourceType.Profiles => serviceProvider.GetService<ProfileOwnershipChecker>(),
            _ => null // No ownership check for other resources
        };
    }
}
