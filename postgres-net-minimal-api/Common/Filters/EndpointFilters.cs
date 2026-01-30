using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using postgres_net_minimal_api.Authorization.Services;
using postgres_net_minimal_api.Authorization.Enums;

namespace postgres_net_minimal_api.Common.Filters;

/// <summary>
/// Endpoint filter to extract current user ID from claims (DRY)
/// Eliminates duplicate code in every endpoint
/// </summary>
public class CurrentUserFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var user = httpContext.User;

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Microsoft.AspNetCore.Http.Results.Json(
                new { error = "Invalid or missing user authentication" },
                statusCode: 401);
        }

        // Add userId to HttpContext.Items for easy access
        httpContext.Items["CurrentUserId"] = userId;

        return await next(context);
    }
}

/// <summary>
/// Endpoint filter to check permissions (DRY + SRP)
/// Centralized permission checking logic
/// </summary>
public class PermissionFilter : IEndpointFilter
{
    private readonly ResourceType _resourceType;
    private readonly ActionType _actionType;
    private readonly bool _checkOwnership;
    private readonly string? _resourceIdParameter;

    public PermissionFilter(
        ResourceType resourceType,
        ActionType actionType,
        bool checkOwnership = false,
        string? resourceIdParameter = null)
    {
        _resourceType = resourceType;
        _actionType = actionType;
        _checkOwnership = checkOwnership;
        _resourceIdParameter = resourceIdParameter;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var permissionChecker = httpContext.RequestServices.GetRequiredService<IPermissionChecker>();

        // Get user ID from HttpContext.Items (set by CurrentUserFilter)
        if (httpContext.Items["CurrentUserId"] is not Guid userId)
        {
            return Microsoft.AspNetCore.Http.Results.Unauthorized();
        }

        // Check ownership if required
        if (_checkOwnership && !string.IsNullOrEmpty(_resourceIdParameter))
        {
            // Try to get resource ID from route values or query string
            var resourceIdValue = httpContext.Request.RouteValues[_resourceIdParameter]?.ToString()
                ?? httpContext.Request.Query[_resourceIdParameter].FirstOrDefault();

            if (resourceIdValue is not null && Guid.TryParse(resourceIdValue, out var resourceId))
            {
                var hasPermission = await permissionChecker.HasPermissionAndOwnsResourceAsync(
                    userId,
                    _resourceType,
                    _actionType,
                    resourceId,
                    httpContext.RequestAborted);

                if (!hasPermission)
                {
                    return Microsoft.AspNetCore.Http.Results.Forbid();
                }

                return await next(context);
            }
        }

        // Check permission only
        var hasBasicPermission = await permissionChecker.HasPermissionAsync(
            userId,
            _resourceType,
            _actionType);

        if (!hasBasicPermission)
        {
            return Microsoft.AspNetCore.Http.Results.Forbid();
        }

        return await next(context);
    }
}

/// <summary>
/// Extension methods for applying filters (KISS)
/// Makes filter application simple and readable
/// </summary>
public static class EndpointFilterExtensions
{
    /// <summary>
    /// Adds current user extraction filter
    /// </summary>
    public static RouteHandlerBuilder WithCurrentUser(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<CurrentUserFilter>();
    }

    /// <summary>
    /// Adds permission check filter
    /// </summary>
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        ResourceType resourceType,
        ActionType actionType)
    {
        return builder.AddEndpointFilter(new PermissionFilter(resourceType, actionType));
    }

    /// <summary>
    /// Adds permission check with ownership validation
    /// </summary>
    public static RouteHandlerBuilder RequirePermissionAndOwnership(
        this RouteHandlerBuilder builder,
        ResourceType resourceType,
        ActionType actionType,
        string resourceIdParameter = "id")
    {
        return builder.AddEndpointFilter(new PermissionFilter(
            resourceType,
            actionType,
            checkOwnership: true,
            resourceIdParameter: resourceIdParameter));
    }

    /// <summary>
    /// Gets current user ID from HttpContext.Items
    /// </summary>
    public static Guid? GetCurrentUserId(this HttpContext httpContext)
    {
        return httpContext.Items["CurrentUserId"] as Guid?;
    }
}
