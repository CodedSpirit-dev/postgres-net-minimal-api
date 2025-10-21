using System.ComponentModel.DataAnnotations;

namespace postgres_net_minimal_api.DTOs;

/// <summary>
/// Request DTO for creating a new user (public registration)
/// </summary>
public record CreateUserRequest
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public required string UserName { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string FirstName { get; init; }

    [StringLength(100)]
    public string? MiddleName { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string LastName { get; init; }

    [StringLength(100)]
    public string? MotherMaidenName { get; init; }

    [Required]
    public required DateOnly DateOfBirth { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public required string Password { get; init; }

    // RoleId is NOT included - will be assigned server-side to prevent privilege escalation
}

/// <summary>
/// Request DTO for updating an existing user (Admin only)
/// </summary>
public record UpdateUserRequest
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public required string UserName { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string FirstName { get; init; }

    [StringLength(100)]
    public string? MiddleName { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string LastName { get; init; }

    [StringLength(100)]
    public string? MotherMaidenName { get; init; }

    [Required]
    public required DateOnly DateOfBirth { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; init; }

    [StringLength(100, MinimumLength = 8)]
    public string? Password { get; init; }  // Optional - only update if provided

    [Required]
    public required Guid RoleId { get; init; }  // Admin can change roles
}

/// <summary>
/// Request DTO for creating a new role (Admin only)
/// </summary>
public record CreateRoleRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public required string Name { get; init; }

    [StringLength(200)]
    public string? Description { get; init; }
}
