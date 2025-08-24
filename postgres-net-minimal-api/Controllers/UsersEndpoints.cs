using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Controllers;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");
        
        // List users - Project to anonymous object to avoid circular reference
        group.MapGet("/", async (AppDbContext db) =>
            await db.Users
                .Include(u => u.Role)
                .Select(u => new 
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.MiddleName,
                    u.Email,
                    u.DateOfBirth,
                    u.MotherMaidenName,
                    Role = new 
                    {
                        u.Role.Id,
                        u.Role.Name,
                        u.Role.Description
                    }
                })
                .ToListAsync());
        
        // Obtain user by ID - Project to anonymous object
        group.MapGet("/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var user = await db.Users
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .Select(u => new 
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.MiddleName,
                    u.Email,
                    u.DateOfBirth,
                    u.MotherMaidenName,
                    Role = new 
                    {
                        u.Role.Id,
                        u.Role.Name,
                        u.Role.Description
                    }
                })
                .FirstOrDefaultAsync();
                
            return user is not null ? Results.Ok(user) : Results.NotFound();
        });

        // Update user
        group.MapPut("/{id}", async (Guid id, User inputUser, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound();

            user.FirstName = inputUser.FirstName;
            user.LastName = inputUser.LastName;
            user.Email = inputUser.Email;
            user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(inputUser.HashedPassword);
            user.RoleId = inputUser.RoleId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        //Delete User
        group.MapDelete("/{id}", async (Guid id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}