// Create these DTOs in a new file: DTOs/UserDtos.cs

using postgres_net_minimal_api.Users.Models;

namespace postgres_net_minimal_api.Users.DTOs;

public record UserResponseDto(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    DateOnly? DateOfBirth, // Use DateOnly for birth dates
    string? MotherMaidenName,
    RoleResponseDto Role
);

public record RoleResponseDto(
    Guid Id,
    string Name,
    string? Description
);

// Extension method to convert entities to DTOs
public static class UserExtensions
{
    public static UserResponseDto ToDto(this User user)
    {
        return new UserResponseDto(
            user.Id,
            user.UserName,
            user.FirstName,
            user.LastName,
            user.MiddleName,
            user.Email,
            user.DateOfBirth,
            user.MotherMaidenName,
            new RoleResponseDto(
                user.Role.Id,
                user.Role.Name,
                user.Role.Description
            )
        );
    }
}