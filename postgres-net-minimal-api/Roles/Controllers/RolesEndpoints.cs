using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Models;

public static class RolesEndpoints
{
    public static void MapRolesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/roles")
            .WithTags("Roles")
            .WithOpenApi();

        // Listar roles
        group.MapGet("/", async (AppDbContext db) =>
                await db.UserRoles.ToListAsync())
            .WithName("GetAllRoles")
            .WithSummary("Obtener todos los roles")
            .WithDescription("Retorna una lista de todos los roles disponibles en el sistema")
            .Produces(200)
            .WithOpenApi();

        // Obtener rol por ID
        group.MapGet("/{id}", async (Guid id, AppDbContext db) =>
                await db.UserRoles.FindAsync(id) is { } role ? Results.Ok(role) : Results.NotFound())
            .WithName("GetRoleById")
            .WithSummary("Obtener rol por ID")
            .WithDescription("Retorna un rol específico por su ID único")
            .Produces(200)
            .Produces(404)
            .WithOpenApi();

        // Crear rol
        group.MapPost("/", async (UserRole role, AppDbContext db) =>
            {
                db.UserRoles.Add(role);
                await db.SaveChangesAsync();
                return Results.Created($"/roles/{role.Id}", role);
            })
            .WithName("CreateRole")
            .WithSummary("Crear nuevo rol")
            .WithDescription("Crea un nuevo rol en el sistema")
            .Produces(201)
            .Produces(400)
            .WithOpenApi();
    }
}