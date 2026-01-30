namespace postgres_net_minimal_api.Common.Results;

/// <summary>
/// Result pattern implementation (KISS)
/// Simplifies error handling and makes success/failure explicit
/// Replaces exceptions for expected failures
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? errorMessage)
    {
        if (isSuccess && !string.IsNullOrEmpty(errorMessage))
        {
            throw new InvalidOperationException("Successful result cannot have error message");
        }

        if (!isSuccess && string.IsNullOrEmpty(errorMessage))
        {
            throw new InvalidOperationException("Failed result must have error message");
        }

        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string errorMessage) => new(false, errorMessage);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, null);
    public static Result<TValue> Failure<TValue>(string errorMessage) => new(default!, false, errorMessage);
}

/// <summary>
/// Generic result with value
/// </summary>
public class Result<TValue> : Result
{
    public TValue? Value { get; }

    internal Result(TValue value, bool isSuccess, string? errorMessage)
        : base(isSuccess, errorMessage)
    {
        Value = value;
    }

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}

/// <summary>
/// Extension methods for Result pattern
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps success value to another type
    /// </summary>
    public static Result<TNew> Map<TOld, TNew>(
        this Result<TOld> result,
        Func<TOld, TNew> mapper)
    {
        return result.IsSuccess
            ? Result.Success(mapper(result.Value!))
            : Result.Failure<TNew>(result.ErrorMessage!);
    }

    /// <summary>
    /// Executes action on success
    /// </summary>
    public static Result<T> OnSuccess<T>(
        this Result<T> result,
        Action<T> action)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Executes action on failure
    /// </summary>
    public static Result<T> OnFailure<T>(
        this Result<T> result,
        Action<string> action)
    {
        if (result.IsFailure && result.ErrorMessage is not null)
        {
            action(result.ErrorMessage);
        }
        return result;
    }

    /// <summary>
    /// Returns value or default
    /// </summary>
    public static T? ValueOrDefault<T>(this Result<T> result, T? defaultValue = default)
    {
        return result.IsSuccess ? result.Value : defaultValue;
    }
}
