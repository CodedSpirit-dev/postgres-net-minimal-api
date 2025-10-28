namespace postgres_net_minimal_api.Services;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
    bool ValidateRefreshToken(string refreshToken, string hashedRefreshToken);
}
