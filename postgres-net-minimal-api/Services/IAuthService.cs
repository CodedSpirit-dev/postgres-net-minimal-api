namespace postgres_net_minimal_api.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token
    /// </summary>
    /// <param name="usernameOrEmail">Username or email address</param>
    /// <param name="password">Plain-text password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token if authentication succeeds, null otherwise</returns>
    Task<string?> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);
}

/// <summary>
/// JWT token generation service
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for a user
    /// </summary>
    string GenerateToken(Guid userId, string email, string username, string roleName);
}
