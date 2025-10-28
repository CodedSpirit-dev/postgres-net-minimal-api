using System.ComponentModel.DataAnnotations;
using postgres_net_minimal_api.Services;
using postgres_net_minimal_api.Users.DTOs;

namespace postgres_net_minimal_api.Auth.Controllers;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        // POST /auth/register - Register new user with default "User" role
        group.MapPost("/register", async (
            CreateUserRequest request,
            IUserService userService,
            IAuthService authService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                // Create user with default "User" role
                var user = await userService.CreateUserAsync(request, cancellationToken);

                // Automatically authenticate and return JWT token
                var token = await authService.AuthenticateAsync(
                    request.Email,
                    request.Password,
                    cancellationToken);

                return Results.Created($"/api/users/{user.Id}", new RegisterResponse(
                    Success: true,
                    Message: "User registered successfully",
                    Token: token,
                    User: user));
            }
            catch (InvalidOperationException ex)
            {
                // Handle validation errors (duplicate email/username, weak password, etc.)
                return Results.BadRequest(new RegisterResponse(
                    Success: false,
                    Message: ex.Message,
                    Token: null,
                    User: null));
            }
        })
        .RequireRateLimiting("login") // Same rate limit as login to prevent abuse
        .WithName("Register")
        .WithSummary("User registration")
        .WithDescription("Registers a new user with the default 'User' role. Returns a JWT token on success. Rate limited to 5 attempts per minute.")
        .Produces<RegisterResponse>(201)
        .Produces<RegisterResponse>(400)
        .Produces(429)
        .WithOpenApi();

        // POST /auth/login - Authenticate user and generate JWT token
        group.MapPost("/login", async (
            LoginRequest request,
            IAuthService authService,
            CancellationToken cancellationToken) =>
        {
            var result = await authService.AuthenticateWithUserDataAsync(
                request.UsernameOrEmail,
                request.Password,
                request.RememberMe,
                cancellationToken);

            if (result is null)
            {
                return Results.Json(
                    new { success = false, message = "Invalid username/email or password" },
                    statusCode: 401);
            }

            return Results.Ok(new LoginResponse(
                Success: true,
                Message: "Login successful",
                Token: result.Token,
                RefreshToken: result.RefreshToken,
                ExpiresAt: result.ExpiresAt,
                ExpiresIn: result.ExpiresIn,
                User: result.User));
        })
        .RequireRateLimiting("login") // Rate limit to prevent brute force attacks
        .WithName("Login")
        .WithSummary("User login")
        .WithDescription("Authenticates a user with email/username and password. Returns a JWT token and user data on success. Rate limited to 5 attempts per minute.")
        .Produces<LoginResponse>(200)
        .Produces(401)
        .Produces(429)
        .WithOpenApi();

        // POST /auth/refresh - Refresh access token using refresh token
        group.MapPost("/refresh", async (
            RefreshTokenRequest request,
            IAuthService authService,
            CancellationToken cancellationToken) =>
        {
            var result = await authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (result is null)
            {
                return Results.Json(
                    new { success = false, message = "Invalid or expired refresh token" },
                    statusCode: 401);
            }

            return Results.Ok(new
            {
                success = true,
                token = result.Token,
                expiresAt = result.ExpiresAt,
                expiresIn = result.ExpiresIn
            });
        })
        .WithName("RefreshToken")
        .WithSummary("Refresh access token")
        .WithDescription("Refreshes an expired access token using a valid refresh token. Returns a new access token.")
        .Produces(200)
        .Produces(401)
        .WithOpenApi();

        // POST /auth/logout - Logout endpoint (revokes refresh token)
        group.MapPost("/logout", async (
            IAuthService authService,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Json(
                    new { success = false, message = "Invalid or missing user authentication" },
                    statusCode: 401);
            }

            await authService.RevokeRefreshTokenAsync(userId, cancellationToken);

            return Results.Ok(new { success = true, message = "Logged out successfully" });
        })
        .RequireAuthorization()
        .WithName("Logout")
        .WithSummary("User logout")
        .WithDescription("Logs out the user by revoking their refresh token. The client should also discard the access token.")
        .Produces(200)
        .Produces(401)
        .WithOpenApi();

        // POST /auth/change-password - Change user password
        group.MapPost("/change-password", async (
            ChangePasswordRequest request,
            IAuthService authService,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            // Extract user ID from JWT claims
            var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Json(
                    new { success = false, message = "Invalid or missing user authentication" },
                    statusCode: 401);
            }

            var success = await authService.ChangePasswordAsync(
                userId,
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken);

            if (!success)
            {
                return Results.Json(
                    new { success = false, message = "Current password is incorrect" },
                    statusCode: 400);
            }

            return Results.Ok(new { success = true, message = "Password changed successfully" });
        })
        .RequireAuthorization()
        .WithName("ChangePassword")
        .WithSummary("Change user password")
        .WithDescription("Allows authenticated users to change their password by providing their current password and new password.")
        .Produces(200)
        .Produces(400)
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

    public bool RememberMe { get; init; } = false;
}

/// <summary>
/// Login response DTO
/// </summary>
public record LoginResponse(
    bool Success,
    string Message,
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    int ExpiresIn,
    UserResponseDto User);

/// <summary>
/// Registration response DTO
/// </summary>
public record RegisterResponse(
    bool Success,
    string Message,
    string? Token,
    UserResponseDto? User);