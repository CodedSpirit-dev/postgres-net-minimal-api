namespace postgres_net_minimal_api.Services;

/// <summary>
/// Validation result for password strength checks
/// </summary>
public record ValidationResult(bool IsValid, string? ErrorMessage)
{
    public static ValidationResult Success() => new(true, null);
    public static ValidationResult Failure(string error) => new(false, error);
}

/// <summary>
/// Service for validating password strength requirements
/// </summary>
public interface IPasswordValidator
{
    ValidationResult Validate(string password);
}

/// <summary>
/// Default password validator with standard security requirements
/// </summary>
public class PasswordValidator : IPasswordValidator
{
    private static readonly string[] CommonPasswords =
    [
        "password", "12345678", "password123", "qwerty123",
        "admin123", "letmein", "welcome123"
    ];

    public ValidationResult Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return ValidationResult.Failure("Password is required");

        if (password.Length < 8)
            return ValidationResult.Failure("Password must be at least 8 characters long");

        if (password.Length > 100)
            return ValidationResult.Failure("Password must be less than 100 characters");

        if (!password.Any(char.IsDigit))
            return ValidationResult.Failure("Password must contain at least one digit");

        if (!password.Any(char.IsUpper))
            return ValidationResult.Failure("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            return ValidationResult.Failure("Password must contain at least one lowercase letter");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return ValidationResult.Failure("Password must contain at least one special character");

        foreach (var commonPassword in CommonPasswords)
        {
            if (password.Contains(commonPassword, StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Failure("Password is too common. Please choose a stronger password");
        }

        return ValidationResult.Success();
    }
}
