using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.DTOs;

namespace postgres_net_minimal_api.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IAuthService
{
    private readonly AppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;

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
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var normalizedIdentifier = usernameOrEmail.ToLower();

        // Find user by email or username
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
        var token = _tokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.UserName,
            user.Role.Name);

        // Return token and user data
        return new AuthenticationResult(token, user.ToDto());
    }
}
