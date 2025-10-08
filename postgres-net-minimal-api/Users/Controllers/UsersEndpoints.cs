using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Controllers;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users")
            .WithOpenApi();
        
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
                .ToListAsync())
            .AllowAnonymous()  // ← AGREGADO: Permite acceso público
            .WithName("GetAllUsers")
            .WithSummary("Obtener todos los usuarios")
            .WithDescription("Retorna una lista de todos los usuarios con su información de rol")
            .Produces(200)
            .WithOpenApi();
        
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
        })
        .AllowAnonymous()  // ← AGREGADO: Permite acceso público
        .WithName("GetUserById")
        .WithSummary("Obtener usuario por ID")
        .WithDescription("Retorna un usuario específico por su ID único")
        .Produces(200)
        .Produces(404)
        .WithOpenApi();

        // Create user - Público para registro
        group.MapPost("/", async (User inputUser, AppDbContext db) =>
        {
            // Validar que el rol sea válido (opcional: solo permitir User o Guest para auto-registro)
            var role = await db.UserRoles.FindAsync(inputUser.RoleId);
            if (role is null)
            {
                return Results.BadRequest(new { error = "El rol especificado no existe" });
            }

            // Validar email único
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == inputUser.Email);
            if (existingUser is not null)
            {
                return Results.BadRequest(new { error = "El email ya está registrado" });
            }

            // Validar username único
            var existingUsername = await db.Users.FirstOrDefaultAsync(u => u.UserName == inputUser.UserName);
            if (existingUsername is not null)
            {
                return Results.BadRequest(new { error = "El nombre de usuario ya está en uso" });
            }

            // Hash de la contraseña antes de guardar
            inputUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(inputUser.HashedPassword);
            inputUser.Id = Guid.NewGuid();
            
            db.Users.Add(inputUser);
            await db.SaveChangesAsync();
            
            return Results.Created($"/users/{inputUser.Id}", new 
            {
                inputUser.Id,
                inputUser.FirstName,
                inputUser.LastName,
                inputUser.MiddleName,
                inputUser.Email,
                inputUser.DateOfBirth,
                inputUser.RoleId
            });
        })
        .AllowAnonymous()  // ← CAMBIADO: Permite registro público
        .WithName("CreateUser")
        .WithSummary("Crear nuevo usuario")
        .WithDescription("Crea un nuevo usuario en el sistema (Registro público).")
        .Produces(201)
        .Produces(400)
        .WithOpenApi();

        // Update user - Solo Admin
        group.MapPut("/{id}", async (Guid id, User inputUser, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound();

            user.FirstName = inputUser.FirstName;
            user.LastName = inputUser.LastName;
            user.MiddleName = inputUser.MiddleName;
            user.Email = inputUser.Email;
            user.UserName = inputUser.UserName;
            user.DateOfBirth = inputUser.DateOfBirth;
            user.MotherMaidenName = inputUser.MotherMaidenName;
            
            // Solo actualizar contraseña si se proporciona
            if (!string.IsNullOrEmpty(inputUser.HashedPassword))
            {
                user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(inputUser.HashedPassword);
            }
            
            user.RoleId = inputUser.RoleId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("UpdateUser")
        .WithSummary("Actualizar usuario")
        .WithDescription("Actualiza la información de un usuario existente. Solo usuarios con rol Admin pueden realizar esta acción.")
        .Produces(204)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();
        
        //Delete User - Solo Admin
        group.MapDelete("/{id}", async (Guid id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("DeleteUser")
        .WithSummary("Eliminar usuario")
        .WithDescription("Elimina permanentemente un usuario del sistema. Solo usuarios con rol Admin pueden realizar esta acción.")
        .Produces(204)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();
    }
}