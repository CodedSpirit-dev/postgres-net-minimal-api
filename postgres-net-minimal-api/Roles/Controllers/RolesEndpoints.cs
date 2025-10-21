using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.DTOs;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Controllers;

public static class RolesEndpoints
{
    public static void MapRolesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/roles")
            .WithTags("Roles")
            .WithOpenApi();

        // GET /roles - List all roles
        group.MapGet("/", async (AppDbContext db, CancellationToken cancellationToken) =>
        {
            var roles = await db.UserRoles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Results.Ok(roles);
        })
        .AllowAnonymous()
        .WithName("GetAllRoles")
        .WithSummary("Get all roles")
        .WithDescription("Returns a list of all available roles in the system")
        .Produces<List<UserRole>>(200)
        .WithOpenApi();

        // GET /roles/{id} - Get role by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            AppDbContext db,
            CancellationToken cancellationToken) =>
        {
            var role = await db.UserRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            return role is not null ? Results.Ok(role) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetRoleById")
        .WithSummary("Get role by ID")
        .WithDescription("Returns a specific role by its unique identifier")
        .Produces<UserRole>(200)
        .Produces(404)
        .WithOpenApi();

        // POST /roles - Create role (Admin only)
        group.MapPost("/", async (
            CreateRoleRequest request,
            AppDbContext db,
            CancellationToken cancellationToken) =>
        {
            // Check if role name already exists
            var nameExists = await db.UserRoles
                .AsNoTracking()
                .AnyAsync(r => r.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (nameExists)
            {
                return Results.BadRequest(new { error = "Role name already exists" });
            }

            var role = new UserRole
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            db.UserRoles.Add(role);
            await db.SaveChangesAsync(cancellationToken);

            return Results.Created($"/roles/{role.Id}", role);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin")) // SECURITY: Only Admin can create roles
        .WithName("CreateRole")
        .WithSummary("Create new role")
        .WithDescription("Creates a new role in the system. Only users with Admin role can perform this action.")
        .Produces<UserRole>(201)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .WithOpenApi();
    }
}