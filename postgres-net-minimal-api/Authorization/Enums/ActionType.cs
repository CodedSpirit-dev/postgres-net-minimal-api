namespace postgres_net_minimal_api.Authorization.Enums;

/// <summary>
/// Defines the actions that can be performed on resources
/// </summary>
public enum ActionType
{
    View,
    Create,
    Edit,
    Delete,

    // Special actions
    Approve,
    Reject,
    Assign,
    Export,
    Import,

    // Administrative
    Manage
}
