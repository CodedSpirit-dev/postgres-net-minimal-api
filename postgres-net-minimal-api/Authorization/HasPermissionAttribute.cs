using Microsoft.AspNetCore.Authorization;
using postgres_net_minimal_api.Authorization.Enums;

namespace postgres_net_minimal_api.Authorization;

/// <summary>
/// Attribute to require a specific permission on an endpoint
/// Example: [HasPermission(ResourceType.Posts, ActionType.Create)]
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute(ResourceType resource, ActionType action) : AuthorizeAttribute
{
    public ResourceType Resource { get; } = resource;
    public ActionType Action { get; } = action;

    /// <summary>
    /// Gets the policy name for this permission
    /// Format: "Permission.{Resource}.{Action}"
    /// </summary>
    public string GetPolicyName() => $"Permission.{Resource}.{Action}";
}
