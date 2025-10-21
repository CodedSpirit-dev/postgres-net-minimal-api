using Microsoft.AspNetCore.Authorization;
using postgres_net_minimal_api.Authorization.Enums;

namespace postgres_net_minimal_api.Authorization;

/// <summary>
/// Authorization requirement that checks both permission and resource ownership
/// Used for instance-level permissions (e.g., "user can only edit their own posts")
/// </summary>
public class ResourceOwnershipRequirement(ResourceType resource, ActionType action) : IAuthorizationRequirement
{
    public ResourceType Resource { get; } = resource;
    public ActionType Action { get; } = action;
}
