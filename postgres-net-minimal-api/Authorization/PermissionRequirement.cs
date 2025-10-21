using Microsoft.AspNetCore.Authorization;
using postgres_net_minimal_api.Authorization.Enums;

namespace postgres_net_minimal_api.Authorization;

/// <summary>
/// Authorization requirement for granular permissions
/// Requires a specific Resource + Action combination
/// </summary>
public class PermissionRequirement(ResourceType resource, ActionType action) : IAuthorizationRequirement
{
    public ResourceType Resource { get; } = resource;
    public ActionType Action { get; } = action;
}
