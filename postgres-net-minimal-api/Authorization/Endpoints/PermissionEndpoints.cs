using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Authorization.DTOs;
using postgres_net_minimal_api.Authorization.Enums;
using postgres_net_minimal_api.Authorization.Services;
using postgres_net_minimal_api.Data;

namespace postgres_net_minimal_api.Authorization.Endpoints;

/// <summary>
/// Endpoints for managing granular permissions
/// </summary>
public static class PermissionEndpoints
{
    public static void MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/permissions")
            .WithTags("Permissions")
            .RequireAuthorization();

        // Get all modules
        group.MapGet("/modules", GetModules)
            .WithName("GetModules")
            .WithSummary("Get all application modules");

        // Get all features
        group.MapGet("/features", GetFeatures)
            .WithName("GetFeatures")
            .WithSummary("Get all features/resources");

        // Get all actions
        group.MapGet("/actions", GetActions)
            .WithName("GetActions")
            .WithSummary("Get all permission actions");

        // Get all feature-actions (available permissions)
        group.MapGet("/feature-actions", GetFeatureActions)
            .WithName("GetFeatureActions")
            .WithSummary("Get all available permissions (feature-action combinations)");

        // Get permissions for a specific role
        group.MapGet("/roles/{roleId:guid}", GetRolePermissions)
            .WithName("GetRolePermissions")
            .WithSummary("Get all permissions assigned to a role");

        // Assign a permission to a role
        group.MapPost("/roles/{roleId:guid}/assign", AssignPermissionToRole)
            .WithName("AssignPermissionToRole")
            .WithSummary("Assign a permission to a role");

        // Remove a permission from a role
        group.MapDelete("/roles/{roleId:guid}/remove/{featureActionId:int}", RemovePermissionFromRole)
            .WithName("RemovePermissionFromRole")
            .WithSummary("Remove a permission from a role");

        // Get current user's permissions
        group.MapGet("/my-permissions", GetMyPermissions)
            .WithName("GetMyPermissions")
            .WithSummary("Get current user's permissions");

        // Check if current user has a specific permission
        group.MapGet("/check", CheckPermission)
            .WithName("CheckPermission")
            .WithSummary("Check if current user has a specific permission");
    }

    private static async Task<IResult> GetModules(AppDbContext context, CancellationToken cancellationToken)
    {
        var modules = await context.ApplicationModules
            .AsNoTracking()
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new ModuleDto(m.Id, m.Name, m.Description, m.IsActive, m.DisplayOrder))
            .ToListAsync(cancellationToken);

        return Results.Ok(modules);
    }

    private static async Task<IResult> GetFeatures(AppDbContext context, CancellationToken cancellationToken)
    {
        var features = await context.Features
            .AsNoTracking()
            .OrderBy(f => f.DisplayOrder)
            .Select(f => new FeatureDto(f.Id, f.Name, f.ResourceKey, f.Description, f.DisplayOrder))
            .ToListAsync(cancellationToken);

        return Results.Ok(features);
    }

    private static async Task<IResult> GetActions(AppDbContext context, CancellationToken cancellationToken)
    {
        var actions = await context.PermissionActions
            .AsNoTracking()
            .OrderBy(a => a.DisplayOrder)
            .Select(a => new ActionDto(a.Id, a.Name, a.ActionKey, a.Description, a.DisplayOrder))
            .ToListAsync(cancellationToken);

        return Results.Ok(actions);
    }

    private static async Task<IResult> GetFeatureActions(AppDbContext context, CancellationToken cancellationToken)
    {
        var featureActions = await context.FeatureActions
            .AsNoTracking()
            .Include(fa => fa.Feature)
            .Include(fa => fa.Action)
            .OrderBy(fa => fa.Feature.DisplayOrder)
            .ThenBy(fa => fa.Action.DisplayOrder)
            .Select(fa => new FeatureActionDto(
                fa.Id,
                fa.FeatureId,
                fa.Feature.Name,
                fa.Feature.ResourceKey,
                fa.ActionId,
                fa.Action.Name,
                fa.Action.ActionKey,
                fa.IsEnabled))
            .ToListAsync(cancellationToken);

        return Results.Ok(featureActions);
    }

    private static async Task<IResult> GetRolePermissions(
        Guid roleId,
        IPermissionService permissionService,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        // Check if role exists
        var role = await context.UserRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role == null)
        {
            return Results.NotFound(new { message = "Role not found" });
        }

        var permissions = await permissionService.GetRolePermissionsAsync(roleId, cancellationToken);

        var permissionDtos = permissions.Select(rp => new RolePermissionDto(
            rp.Id,
            rp.RoleId,
            role.Name,
            rp.FeatureActionId,
            rp.FeatureAction.Feature.Name,
            rp.FeatureAction.Feature.ResourceKey,
            rp.FeatureAction.Action.Name,
            rp.FeatureAction.Action.ActionKey,
            rp.CreatedAt
        )).ToList();

        return Results.Ok(permissionDtos);
    }

    private static async Task<IResult> AssignPermissionToRole(
        Guid roleId,
        [FromBody] AssignPermissionRequest request,
        IPermissionService permissionService,
        CancellationToken cancellationToken)
    {
        var success = await permissionService.AssignPermissionToRoleAsync(
            roleId,
            request.FeatureActionId,
            cancellationToken);

        if (!success)
        {
            return Results.BadRequest(new { message = "Failed to assign permission. It may already be assigned or the role/permission does not exist." });
        }

        return Results.Ok(new { message = "Permission assigned successfully" });
    }

    private static async Task<IResult> RemovePermissionFromRole(
        Guid roleId,
        int featureActionId,
        IPermissionService permissionService,
        CancellationToken cancellationToken)
    {
        var success = await permissionService.RemovePermissionFromRoleAsync(
            roleId,
            featureActionId,
            cancellationToken);

        if (!success)
        {
            return Results.NotFound(new { message = "Permission assignment not found" });
        }

        return Results.Ok(new { message = "Permission removed successfully" });
    }

    private static async Task<IResult> GetMyPermissions(
        HttpContext httpContext,
        IPermissionService permissionService,
        AppDbContext context)
    {
        var userIdClaim = httpContext.User.FindFirst("UserId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Results.Unauthorized();
        }

        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Results.NotFound(new { message = "User not found" });
        }

        var permissions = await permissionService.GetUserPermissionsAsync(userId);

        var response = new UserPermissionsDto(
            user.Id,
            user.UserName,
            user.RoleId,
            user.Role.Name,
            permissions
        );

        return Results.Ok(response);
    }

    private static async Task<IResult> CheckPermission(
        HttpContext httpContext,
        [FromQuery] string resource,
        [FromQuery] string action,
        IPermissionService permissionService)
    {
        var userIdClaim = httpContext.User.FindFirst("UserId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Results.Unauthorized();
        }

        // Parse the resource and action enums
        if (!Enum.TryParse<ResourceType>(resource, true, out var resourceType) ||
            !Enum.TryParse<ActionType>(action, true, out var actionType))
        {
            return Results.BadRequest(new { message = "Invalid resource or action" });
        }

        var hasPermission = await permissionService.UserHasPermissionAsync(userId, resourceType, actionType);

        return Results.Ok(new { hasPermission, resource, action });
    }
}
