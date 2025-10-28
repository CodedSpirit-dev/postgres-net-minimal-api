using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Users.DTOs;
using postgres_net_minimal_api.Users.Models;
using postgres_net_minimal_api.Roles.Models;
using postgres_net_minimal_api.Helpers;

namespace postgres_net_minimal_api.Services;

/// <summary>
/// Service implementation for user management operations
/// </summary>
public class UserService(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    IPasswordValidator passwordValidator) : IUserService
{
    private readonly AppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IPasswordValidator _passwordValidator = passwordValidator;

    public async Task<PagedResult<UserResponseDto>> GetAllUsersAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100; // Max page size to prevent abuse

        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.Role);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var users = await query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => u.ToDto())
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponseDto>(
            users,
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user?.ToDto();
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Validate password strength
        var passwordValidation = _passwordValidator.Validate(request.Password);
        if (!passwordValidation.IsValid)
        {
            throw new InvalidOperationException(passwordValidation.ErrorMessage);
        }

        // Validate email uniqueness
        var emailExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email address is already registered");
        }

        // Validate username uniqueness
        var usernameExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserName.ToLower() == request.UserName.ToLower(), cancellationToken);

        if (usernameExists)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        // Get the default "User" role (prevent privilege escalation)
        var defaultRole = await _context.UserRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);

        if (defaultRole is null)
        {
            throw new InvalidOperationException("Default user role not found. Please contact system administrator");
        }

        // Create new user entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            MotherMaidenName = request.MotherMaidenName,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            HashedPassword = _passwordHasher.HashPassword(request.Password),
            RoleId = defaultRole.Id
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with role for DTO conversion
        var createdUser = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstAsync(u => u.Id == user.Id, cancellationToken);

        return createdUser.ToDto();
    }

    public async Task<UserResponseDto?> UpdateUserAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        // Validate email uniqueness (excluding current user)
        var emailExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != id, cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email address is already registered");
        }

        // Validate username uniqueness (excluding current user)
        var usernameExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserName.ToLower() == request.UserName.ToLower() && u.Id != id, cancellationToken);

        if (usernameExists)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        // Validate role exists
        var roleExists = await _context.UserRoles
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

        if (!roleExists)
        {
            throw new InvalidOperationException("The specified role does not exist");
        }

        // Update user properties
        user.UserName = request.UserName;
        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;
        user.MotherMaidenName = request.MotherMaidenName;
        user.DateOfBirth = request.DateOfBirth;
        user.Email = request.Email;
        user.RoleId = request.RoleId;

        // Only update password if provided
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var passwordValidation = _passwordValidator.Validate(request.Password);
            if (!passwordValidation.IsValid)
            {
                throw new InvalidOperationException(passwordValidation.ErrorMessage);
            }

            user.HashedPassword = _passwordHasher.HashPassword(request.Password);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload to get updated role
        await _context.Entry(user).Reference(u => u.Role).LoadAsync(cancellationToken);

        return user.ToDto();
    }

    public async Task<UserResponseDto?> UpdateMyProfileAsync(
        Guid userId,
        UpdateMyProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        // Validate email uniqueness (excluding current user)
        var emailExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != userId, cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email address is already registered");
        }

        // Validate username uniqueness (excluding current user)
        var usernameExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserName.ToLower() == request.UserName.ToLower() && u.Id != userId, cancellationToken);

        if (usernameExists)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        // Update user properties (NOT role - users cannot change their own role)
        user.UserName = request.UserName;
        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;
        user.MotherMaidenName = request.MotherMaidenName;
        user.DateOfBirth = request.DateOfBirth;
        user.Email = request.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([id], cancellationToken);

        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
