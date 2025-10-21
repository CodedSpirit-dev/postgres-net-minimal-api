using System.ComponentModel.DataAnnotations;
using postgres_net_minimal_api.Services;

namespace postgres_net_minimal_api.Controllers;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        // POST /auth/login - Authenticate user and generate JWT token
        group.MapPost("/login", async (
            LoginRequest request,
            IAuthService authService,
            CancellationToken cancellationToken) =>
        {
            var token = await authService.AuthenticateAsync(
                request.UsernameOrEmail,
                request.Password,
                cancellationToken);

            if (token is null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new LoginResponse(token));
        })
        .RequireRateLimiting("login") // Rate limit to prevent brute force attacks
        .WithName("Login")
        .WithSummary("User login")
        .WithDescription("Authenticates a user with email/username and password. Returns a JWT token on success. Rate limited to 5 attempts per minute.")
        .Produces<LoginResponse>(200)
        .Produces(401)
        .Produces(429)
        .WithOpenApi();

        // POST /auth/logout - Logout endpoint (placeholder for token blacklisting in production)
        group.MapPost("/logout", () =>
            Results.Ok(new { Message = "Logout successful. Please discard your token client-side." }))
        .RequireAuthorization()
        .WithName("Logout")
        .WithSummary("User logout")
        .WithDescription("Logout endpoint. In a stateless JWT system, the client should discard the token. In production, implement token blacklisting.")
        .Produces(200)
        .Produces(401)
        .WithOpenApi();
    }
}

/// <summary>
/// Login request DTO
/// </summary>
public record LoginRequest
{
    [Required]
    [StringLength(255, MinimumLength = 3)]
    public required string UsernameOrEmail { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public required string Password { get; init; }
}

/// <summary>
/// Login response DTO
/// </summary>
public record LoginResponse(string Token);