using postgres_net_minimal_api.DTOs;

namespace postgres_net_minimal_api.Services;

/// <summary>
/// Service interface for user management operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves all users with pagination support
    /// </summary>
    Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    Task<UserResponseDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user with the default "User" role
    /// </summary>
    Task<UserResponseDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates own profile (User can edit their own data, except role)
    /// </summary>
    Task<UserResponseDto?> UpdateMyProfileAsync(Guid userId, UpdateMyProfileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user (Admin only - can change role)
    /// </summary>
    Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user permanently (Admin only)
    /// </summary>
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a paginated result set
/// </summary>
public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
