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
        var group = app.MapGroup("/roles");

        // Listar roles
        group.MapGet("/", async (AppDbContext db) =>
            await db.UserRoles.ToListAsync());

        // Obtener rol por ID
        group.MapGet("/{id}", async (Guid id, AppDbContext db) =>
            await db.UserRoles.FindAsync(id) is { } role ? Results.Ok(role) : Results.NotFound());

        // Crear rol
        group.MapPost("/", async (UserRole role, AppDbContext db) =>
        {
            db.UserRoles.Add(role);
            await db.SaveChangesAsync();
            return Results.Created($"/roles/{role.Id}", role);
        });
    }
}