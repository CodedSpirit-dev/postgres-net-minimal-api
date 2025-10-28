using postgres_net_minimal_api.Users.DTOs;

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

    /// <summary>
    /// Authenticates a user and returns both JWT token and user data
    /// </summary>
    /// <param name="usernameOrEmail">Username or email address</param>
    /// <param name="password">Plain-text password</param>
    /// <param name="rememberMe">Whether to extend refresh token expiration for "Remember Me" feature</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with token and user data, null if authentication fails</returns>
    Task<AuthenticationResult?> AuthenticateWithUserDataAsync(string usernameOrEmail, string password, bool rememberMe = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currentPassword">Current password for verification</param>
    /// <param name="newPassword">New password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if password changed successfully, false if current password is incorrect</returns>
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token if refresh succeeds, null otherwise</returns>
    Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a user's refresh token (logout)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Authentication result containing token and user data
/// </summary>
public record AuthenticationResult(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    int ExpiresIn,
    UserResponseDto User);

public record RefreshTokenRequest(string RefreshToken);

public record RefreshTokenResponse(
    string Token,
    DateTime ExpiresAt,
    int ExpiresIn);

/// <summary>
/// JWT token generation service
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for a user
    /// </summary>
    string GenerateToken(Guid userId, string email, string username, string roleName);

    /// <summary>
    /// Gets the expiration date for access tokens
    /// </summary>
    DateTime GetTokenExpiration();

    /// <summary>
    /// Gets the expiration date for refresh tokens
    /// </summary>
    DateTime GetRefreshTokenExpiration();

    /// <summary>
    /// Gets the expiration date for refresh tokens based on Remember Me preference
    /// </summary>
    /// <param name="rememberMe">Whether to use extended expiration (Remember Me)</param>
    DateTime GetRefreshTokenExpiration(bool rememberMe);
}
