using Microsoft.AspNetCore.Authorization;
using postgres_net_minimal_api.Authorization.Services;

namespace postgres_net_minimal_api.Authorization;

/// <summary>
/// Authorization handler that checks if a user has the required permission
/// Uses the PermissionService to check permissions with caching
/// </summary>
public class PermissionAuthorizationHandler(IPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Get user ID from claims
        var userIdClaim = context.User.FindFirst("UserId") ?? context.User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return; // User not authenticated or invalid user ID
        }

        // Check if user has the required permission
        var hasPermission = await permissionService.UserHasPermissionAsync(
            userId,
            requirement.Resource,
            requirement.Action);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
