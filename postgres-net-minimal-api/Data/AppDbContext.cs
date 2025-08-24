using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Data;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // GUIDs fijos para roles
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Semillas para roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = adminRoleId, Name = "Admin", Description = "Administrador del sistema" },
            new UserRole { Id = userRoleId, Name = "User", Description = "Usuario estándar" },
            new UserRole { Id = guestRoleId, Name = "Guest", Description = "Invitado, acceso limitado" }
        );

        // GUIDs fijos para usuarios
        var adminUserId = Guid.Parse("A1111111-1111-1111-1111-111111111111");
        var standardUserId = Guid.Parse("B2222222-2222-2222-2222-222222222222");

        // Reemplaza BCrypt.Net.BCrypt.HashPassword con hashes estáticos pre-generados
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
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
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                HashedPassword = "yo$2a$11$5ByfQRylb6t1ucfmqMASC.OGbS4Qp7sPq4Dpc1YC24oiG6usM26PK",
                DateOfBirth = new DateOnly(1995, 5, 15),
                RoleId = userRoleId
            }
        );
    }
}