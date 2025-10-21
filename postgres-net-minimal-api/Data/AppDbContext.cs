using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Data;

/// <summary>
/// Database context for the application using PostgreSQL
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fixed GUIDs for roles
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Seed data for roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = adminRoleId, Name = "Admin", Description = "System administrator" },
            new UserRole { Id = userRoleId, Name = "User", Description = "Standard user" },
            new UserRole { Id = guestRoleId, Name = "Guest", Description = "Guest user with limited access" }
        );

        // Fixed GUIDs for users
        var adminUserId = Guid.Parse("A1111111-1111-1111-1111-111111111111");
        var standardUserId = Guid.Parse("B2222222-2222-2222-2222-222222222222");

        // Pre-generated BCrypt hashes for seed data
        // Admin password: AdminPassword123!
        // User password: UserPassword123!
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "System",
                Email = "admin@example.com",
                HashedPassword = "$2a$11$67rVUGbvftj.YcqUIiQGSeg47kWVtGJXZR8ZbESPh7VG5glAAtDqe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                RoleId = adminRoleId
            },
            new User
            {
                Id = standardUserId,
                UserName = "User",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                HashedPassword = "$2a$11$5ByfQRylb6t1ucfmqMASC.OGbS4Qp7sPq4Dpc1YC24oiG6usM26PK",
                DateOfBirth = new DateOnly(1995, 5, 15),
                RoleId = userRoleId
            }
        );

        // Add composite index for common queries
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.RoleId, u.LastName })
            .HasDatabaseName("IX_Users_RoleId_LastName");
    }
}