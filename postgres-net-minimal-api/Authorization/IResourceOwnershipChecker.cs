namespace postgres_net_minimal_api.Authorization;

/// <summary>
/// Interface for checking if a user owns a specific resource instance
/// Implement this for each resource type that needs ownership validation
/// </summary>
public interface IResourceOwnershipChecker
{
    /// <summary>
    /// Check if a user owns a specific resource
    /// </summary>
    /// <param name="userId">The user ID to check</param>
    /// <param name="resourceId">The ID of the resource (post, comment, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the user owns the resource, false otherwise</returns>
    Task<bool> UserOwnsResourceAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken = default);
}
