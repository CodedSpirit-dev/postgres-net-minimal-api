using System.Security.Cryptography;

namespace postgres_net_minimal_api.Services;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashRefreshToken(string refreshToken)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hashBytes);
    }

    public bool ValidateRefreshToken(string refreshToken, string hashedRefreshToken)
    {
        var computedHash = HashRefreshToken(refreshToken);
        return computedHash == hashedRefreshToken;
    }
}
