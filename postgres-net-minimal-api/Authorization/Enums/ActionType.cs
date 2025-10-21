namespace postgres_net_minimal_api.Authorization.Enums;

/// <summary>
/// Defines the actions that can be performed on resources
/// </summary>
public enum ActionType
{
    // Basic CRUD
    View,
    Create,
    Edit,
    Delete,

    // Publishing actions (for blog posts)
    Publish,
    Unpublish,

    // Moderation actions (for comments, posts)
    Approve,
    Reject,
    Moderate,

    // Data transfer
    Assign,
    Export,
    Import,

    // Administrative
    Manage
}
