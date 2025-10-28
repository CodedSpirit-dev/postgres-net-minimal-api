using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Users.DTOs;

namespace postgres_net_minimal_api.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator) : IAuthService
{
    private readonly AppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;

    public async Task<string?> AuthenticateAsync(
        string usernameOrEmail,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var normalizedIdentifier = usernameOrEmail.ToLower();

        // Find user by email or username
        // Note: ToLower() prevents index usage - in production, use case-insensitive collation
        // or PostgreSQL citext type
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Email.ToLower() == normalizedIdentifier ||
                u.UserName.ToLower() == normalizedIdentifier,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.HashedPassword))
        {
            return null;
        }

        // Generate JWT token
        return _tokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.UserName,
            user.Role.Name);
    }

    public async Task<AuthenticationResult?> AuthenticateWithUserDataAsync(
        string usernameOrEmail,
        string password,
        bool rememberMe = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var normalizedIdentifier = usernameOrEmail.ToLower();

        // Find user by email or username
        var user = await _context.Users
            .AsTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Email.ToLower() == normalizedIdentifier ||
                u.UserName.ToLower() == normalizedIdentifier,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.HashedPassword))
        {
            return null;
        }

        // Generate JWT token
        var token = _tokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.UserName,
            user.Role.Name);

        // Generate refresh token
        var refreshToken = _refreshTokenGenerator.GenerateRefreshToken();
        var hashedRefreshToken = _refreshTokenGenerator.HashRefreshToken(refreshToken);

        // Save refresh token to database
        user.RefreshToken = hashedRefreshToken;
        user.RefreshTokenExpiryTime = _tokenGenerator.GetRefreshTokenExpiration(rememberMe);
        await _context.SaveChangesAsync(cancellationToken);

        // Get expiration info
        var expiresAt = _tokenGenerator.GetTokenExpiration();
        var expiresIn = (int)(expiresAt - DateTime.UtcNow).TotalSeconds;

        // Return token and user data
        return new AuthenticationResult(token, refreshToken, expiresAt, expiresIn, user.ToDto());
    }

    public async Task<bool> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            return false;
        }

        // Find user
        var user = await _context.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        // Verify current password
        if (!_passwordHasher.VerifyPassword(currentPassword, user.HashedPassword))
        {
            return false;
        }

        // Hash new password
        user.HashedPassword = _passwordHasher.HashPassword(newPassword);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<RefreshTokenResponse?> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var hashedRefreshToken = _refreshTokenGenerator.HashRefreshToken(refreshToken);

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken, cancellationToken);

        if (user is null)
        {
            return null;
        }

        if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        var newToken = _tokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.UserName,
            user.Role.Name);

        var expiresAt = _tokenGenerator.GetTokenExpiration();
        var expiresIn = (int)(expiresAt - DateTime.UtcNow).TotalSeconds;

        return new RefreshTokenResponse(newToken, expiresAt, expiresIn);
    }

    public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is not null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
