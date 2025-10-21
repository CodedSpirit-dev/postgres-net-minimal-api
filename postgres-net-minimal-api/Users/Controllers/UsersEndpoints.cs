using postgres_net_minimal_api.DTOs;
using postgres_net_minimal_api.Services;

namespace postgres_net_minimal_api.Controllers;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users")
            .WithOpenApi();

        // GET /users - Get all users with pagination
        group.MapGet("/", async (
            IUserService userService,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default) =>
        {
            var result = await userService.GetAllUsersAsync(page, pageSize, cancellationToken);
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .WithName("GetAllUsers")
        .WithSummary("Get all users")
        .WithDescription("Returns a paginated list of all users with their role information")
        .Produces<PagedResult<UserResponseDto>>(200)
        .WithOpenApi();
        
        // GET /users/{id} - Get user by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {
            var user = await userService.GetUserByIdAsync(id, cancellationToken);
            return user is not null ? Results.Ok(user) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetUserById")
        .WithSummary("Get user by ID")
        .WithDescription("Returns a specific user by their unique identifier")
        .Produces<UserResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // POST /users - Create user (public registration)
        group.MapPost("/", async (
            CreateUserRequest request,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var user = await userService.CreateUserAsync(request, cancellationToken);
                return Results.Created($"/users/{user.Id}", user);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .AllowAnonymous()
        .WithName("CreateUser")
        .WithSummary("Create new user")
        .WithDescription("Creates a new user in the system (Public registration). User is assigned the default 'User' role automatically.")
        .Produces<UserResponseDto>(201)
        .Produces(400)
        .WithOpenApi();

        // PUT /users/me - Update own profile (authenticated users)
        group.MapPut("/me", async (
            UpdateMyProfileRequest request,
            IUserService userService,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            // Extract user ID from JWT claims
            var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Json(
                    new { error = "Invalid or missing user authentication" },
                    statusCode: 401);
            }

            try
            {
                var user = await userService.UpdateMyProfileAsync(userId, request, cancellationToken);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization()
        .WithName("UpdateMyProfile")
        .WithSummary("Update own profile")
        .WithDescription("Allows authenticated users to update their own profile information (username, name, email, date of birth). Users cannot change their own role.")
        .Produces<UserResponseDto>(200)
        .Produces(400)
        .Produces(401)
        .Produces(404)
        .WithOpenApi();

        // PUT /users/{id} - Update user (Admin only)
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var user = await userService.UpdateUserAsync(id, request, cancellationToken);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("UpdateUser")
        .WithSummary("Update user")
        .WithDescription("Updates an existing user's information. Only users with Admin role can perform this action.")
        .Produces<UserResponseDto>(200)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();
        
        // DELETE /users/{id} - Delete user (Admin only)
        group.MapDelete("/{id:guid}", async (
            Guid id,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {
            var deleted = await userService.DeleteUserAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("DeleteUser")
        .WithSummary("Delete user")
        .WithDescription("Permanently deletes a user from the system. Only users with Admin role can perform this action.")
        .Produces(204)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();
    }
}