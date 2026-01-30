using postgres_net_minimal_api.Common.Results;

namespace postgres_net_minimal_api.Common.Validation;

/// <summary>
/// Validator interface (SRP + OCP)
/// Each validator has a single responsibility
/// Easy to extend with new validators without modifying existing code
/// </summary>
public interface IValidator<in T>
{
    Result Validate(T value);
}

/// <summary>
/// Async validator for operations requiring database access
/// </summary>
public interface IAsyncValidator<in T>
{
    Task<Result> ValidateAsync(T value, CancellationToken cancellationToken = default);
}

/// <summary>
/// Composite validator (Composite Pattern + OCP)
/// Combines multiple validators without modifying them
/// </summary>
public class CompositeValidator<T> : IValidator<T>
{
    private readonly List<IValidator<T>> _validators = [];

    public CompositeValidator<T> AddValidator(IValidator<T> validator)
    {
        _validators.Add(validator);
        return this;
    }

    public Result Validate(T value)
    {
        foreach (var validator in _validators)
        {
            var result = validator.Validate(value);
            if (result.IsFailure)
            {
                return result;
            }
        }
        return Result.Success();
    }
}

/// <summary>
/// Composite async validator
/// </summary>
public class CompositeAsyncValidator<T> : IAsyncValidator<T>
{
    private readonly List<IAsyncValidator<T>> _validators = [];

    public CompositeAsyncValidator<T> AddValidator(IAsyncValidator<T> validator)
    {
        _validators.Add(validator);
        return this;
    }

    public async Task<Result> ValidateAsync(T value, CancellationToken cancellationToken = default)
    {
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(value, cancellationToken);
            if (result.IsFailure)
            {
                return result;
            }
        }
        return Result.Success();
    }
}
