using postgres_net_minimal_api.Common.Results;

namespace postgres_net_minimal_api.Services;

/// <summary>
/// Validation result for password strength checks (LEGACY - Use Common.Results.Result instead)
/// Kept for backward compatibility
/// </summary>
[Obsolete("Use postgres_net_minimal_api.Common.Results.Result instead")]
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
/// Now uses the new Result pattern (SOLID principles applied)
/// </summary>
public class PasswordValidator : IPasswordValidator
{
    private readonly Common.Validation.PasswordStrengthValidator _strengthValidator;
    private readonly string[] _commonPasswords =
    [
        "password", "12345678", "password123", "qwerty123",
        "admin123", "letmein", "welcome123"
    ];

    public PasswordValidator()
    {
        _strengthValidator = new Common.Validation.PasswordStrengthValidator();
    }

    public ValidationResult Validate(string password)
    {
        // Use new validator system
        var result = _strengthValidator.Validate(password);

        if (result.IsFailure)
        {
            return ValidationResult.Failure(result.ErrorMessage!);
        }

        // Additional check for common weak passwords
        if (_commonPasswords.Any(weak => password.ToLower().Contains(weak)))
        {
            return ValidationResult.Failure("Password is too common. Please choose a stronger password");
        }

        return ValidationResult.Success();
    }
}
