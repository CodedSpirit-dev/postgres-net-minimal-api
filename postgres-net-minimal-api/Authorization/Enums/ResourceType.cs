namespace postgres_net_minimal_api.Authorization.Enums;

/// <summary>
/// Defines the resources (features) that can have permissions
/// </summary>
public enum ResourceType
{
    // User Management
    Users,
    Roles,
    Profiles,

    // Blog Management
    Posts,
    Categories,
    Tags,
    Comments,

    // System
    Permissions,
    Statistics,
    Settings,

    // Future extensibility
    Reports,
    Analytics,
    Audit
}
