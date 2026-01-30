using postgres_net_minimal_api.Common.Specifications;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Users.Specifications;

/// <summary>
/// Specification for users with their roles (OCP + SRP)
/// Encapsulates the query logic for loading users with roles
/// Can be reused across different queries
/// </summary>
public class UsersWithRolesSpecification : BaseSpecification<User>
{
    public UsersWithRolesSpecification() : base()
    {
        AddInclude(u => u.Role);
    }

    public UsersWithRolesSpecification(Guid userId) : base(u => u.Id == userId)
    {
        AddInclude(u => u.Role);
    }
}

/// <summary>
/// Specification for paginated users ordered by name
/// </summary>
public class PaginatedUsersSpecification : BaseSpecification<User>
{
    public PaginatedUsersSpecification(int page, int pageSize) : base()
    {
        AddInclude(u => u.Role);
        ApplyOrderBy(u => u.LastName);
        ApplyPaging((page - 1) * pageSize, pageSize);
    }
}

/// <summary>
/// Specification to find user by email or username
/// </summary>
public class UserByEmailOrUsernameSpecification : BaseSpecification<User>
{
    public UserByEmailOrUsernameSpecification(string identifier) : base(u =>
        u.Email.ToLower() == identifier.ToLower() ||
        u.UserName.ToLower() == identifier.ToLower())
    {
        AddInclude(u => u.Role);
    }
}

/// <summary>
/// Specification to check if email exists (excluding specific user)
/// </summary>
public class EmailExistsSpecification : BaseSpecification<User>
{
    public EmailExistsSpecification(string email, Guid? excludeUserId = null)
        : base(u => u.Email.ToLower() == email.ToLower() && (excludeUserId == null || u.Id != excludeUserId))
    {
    }
}

/// <summary>
/// Specification to check if username exists (excluding specific user)
/// </summary>
public class UsernameExistsSpecification : BaseSpecification<User>
{
    public UsernameExistsSpecification(string username, Guid? excludeUserId = null)
        : base(u => u.UserName.ToLower() == username.ToLower() && (excludeUserId == null || u.Id != excludeUserId))
    {
    }
}
