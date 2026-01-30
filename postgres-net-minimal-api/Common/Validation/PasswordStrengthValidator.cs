using postgres_net_minimal_api.Common.Results;
using System.Text.RegularExpressions;

namespace postgres_net_minimal_api.Common.Validation;

/// <summary>
/// Password strength validator (SRP)
/// Single responsibility: validate password strength
/// </summary>
public partial class PasswordStrengthValidator : IValidator<string>
{
    private const int MinLength = 8;
    private const int MaxLength = 100;

    public Result Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Result.Failure("Password is required");
        }

        if (password.Length < MinLength)
        {
            return Result.Failure($"Password must be at least {MinLength} characters long");
        }

        if (password.Length > MaxLength)
        {
            return Result.Failure($"Password cannot exceed {MaxLength} characters");
        }

        if (!HasUpperCase().IsMatch(password))
        {
            return Result.Failure("Password must contain at least one uppercase letter");
        }

        if (!HasLowerCase().IsMatch(password))
        {
            return Result.Failure("Password must contain at least one lowercase letter");
        }

        if (!HasDigit().IsMatch(password))
        {
            return Result.Failure("Password must contain at least one digit");
        }

        if (!HasSpecialChar().IsMatch(password))
        {
            return Result.Failure("Password must contain at least one special character (@$!%*?&#)");
        }

        return Result.Success();
    }

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex HasUpperCase();

    [GeneratedRegex(@"[a-z]")]
    private static partial Regex HasLowerCase();

    [GeneratedRegex(@"\d")]
    private static partial Regex HasDigit();

    [GeneratedRegex(@"[@$!%*?&#]")]
    private static partial Regex HasSpecialChar();
}

/// <summary>
/// Email format validator (SRP)
/// </summary>
public partial class EmailFormatValidator : IValidator<string>
{
    public Result Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure("Email is required");
        }

        if (!EmailRegex().IsMatch(email))
        {
            return Result.Failure("Invalid email format");
        }

        return Result.Success();
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();
}

/// <summary>
/// Username format validator (SRP)
/// </summary>
public partial class UsernameFormatValidator : IValidator<string>
{
    private const int MinLength = 3;
    private const int MaxLength = 50;

    public Result Validate(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Result.Failure("Username is required");
        }

        if (username.Length < MinLength || username.Length > MaxLength)
        {
            return Result.Failure($"Username must be between {MinLength} and {MaxLength} characters");
        }

        if (!UsernameRegex().IsMatch(username))
        {
            return Result.Failure("Username can only contain letters, numbers, underscores, and hyphens");
        }

        return Result.Success();
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_-]+$")]
    private static partial Regex UsernameRegex();
}
