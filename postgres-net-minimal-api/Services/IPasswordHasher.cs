namespace postgres_net_minimal_api.Services;

/// <summary>
/// Interface for password hashing operations
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password using BCrypt
    /// </summary>
    /// <param name="password">Plain-text password</param>
    /// <returns>BCrypt hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain-text password against a BCrypt hash
    /// </summary>
    /// <param name="password">Plain-text password</param>
    /// <param name="hash">BCrypt hash to verify against</param>
    /// <returns>True if password matches hash, false otherwise</returns>
    bool VerifyPassword(string password, string hash);
}

/// <summary>
/// BCrypt implementation of password hasher
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
