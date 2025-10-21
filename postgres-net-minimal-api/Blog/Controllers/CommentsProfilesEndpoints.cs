using System.Security.Claims;
using postgres_net_minimal_api.Blog.Services;
using postgres_net_minimal_api.DTOs;

namespace postgres_net_minimal_api.Blog.Controllers;

public static class CommentsEndpoints
{
    public static void MapCommentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/comments")
            .WithTags("Comments")
            .WithOpenApi();

        // GET /api/comments/post/{postId}
        group.MapGet("/post/{postId:guid}", async (
            Guid postId,
            ICommentService commentService,
            bool onlyApproved = true,
            CancellationToken cancellationToken = default) =>
        {
            var comments = await commentService.GetCommentsByPostIdAsync(postId, onlyApproved, cancellationToken);
            return Results.Ok(comments);
        })
        .AllowAnonymous()
        .WithName("GetCommentsByPostId")
        .WithSummary("Get comments by post ID")
        .WithDescription("Returns all comments for a specific post, including nested replies")
        .Produces<List<CommentResponseDto>>(200)
        .WithOpenApi();

        // GET /api/comments/{id}
        group.MapGet("/{id:guid}", async (
            Guid id,
            ICommentService commentService,
            CancellationToken cancellationToken) =>
        {
            var comment = await commentService.GetCommentByIdAsync(id, cancellationToken);
            return comment is not null ? Results.Ok(comment) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetCommentById")
        .WithSummary("Get comment by ID")
        .Produces<CommentResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // GET /api/comments/pending - Get pending comments (Admin only)
        group.MapGet("/pending", async (
            ICommentService commentService,
            CancellationToken cancellationToken) =>
        {
            var comments = await commentService.GetPendingCommentsAsync(cancellationToken);
            return Results.Ok(comments);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("GetPendingComments")
        .WithSummary("Get pending comments")
        .WithDescription("Returns all comments awaiting approval. Requires Admin role.")
        .Produces<List<CommentResponseDto>>(200)
        .Produces(401)
        .Produces(403)
        .WithOpenApi();

        // POST /api/comments
        group.MapPost("/", async (
            CreateCommentRequest request,
            ICommentService commentService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var authorId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
                var comment = await commentService.CreateCommentAsync(request, authorId, cancellationToken);
                return Results.Created($"/api/comments/{comment.Id}", comment);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization()
        .WithName("CreateComment")
        .WithSummary("Create new comment")
        .WithDescription("Creates a new comment on a post. Requires authentication. Comments require approval.")
        .Produces<CommentResponseDto>(201)
        .Produces(400)
        .Produces(401)
        .WithOpenApi();

        // PUT /api/comments/{id}
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCommentRequest request,
            ICommentService commentService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var comment = await commentService.UpdateCommentAsync(id, request, cancellationToken);
                return comment is not null ? Results.Ok(comment) : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization()
        .WithName("UpdateComment")
        .WithSummary("Update comment")
        .Produces<CommentResponseDto>(200)
        .Produces(400)
        .Produces(401)
        .Produces(404)
        .WithOpenApi();

        // DELETE /api/comments/{id}
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICommentService commentService,
            CancellationToken cancellationToken) =>
        {
            var deleted = await commentService.DeleteCommentAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("DeleteComment")
        .WithSummary("Delete comment")
        .WithDescription("Permanently deletes a comment. Requires Admin role.")
        .Produces(204)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // POST /api/comments/{id}/approve
        group.MapPost("/{id:guid}/approve", async (
            Guid id,
            ICommentService commentService,
            CancellationToken cancellationToken) =>
        {
            var approved = await commentService.ApproveCommentAsync(id, cancellationToken);
            return approved ? Results.Ok(new { message = "Comment approved" }) : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("ApproveComment")
        .WithSummary("Approve comment")
        .WithDescription("Approves a pending comment. Requires Admin role.")
        .Produces(200)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // POST /api/comments/{id}/reject
        group.MapPost("/{id:guid}/reject", async (
            Guid id,
            ICommentService commentService,
            CancellationToken cancellationToken) =>
        {
            var rejected = await commentService.RejectCommentAsync(id, cancellationToken);
            return rejected ? Results.Ok(new { message = "Comment rejected" }) : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("RejectComment")
        .WithSummary("Reject comment")
        .WithDescription("Rejects an approved comment. Requires Admin role.")
        .Produces(200)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();
    }
}

public static class ProfilesEndpoints
{
    public static void MapProfilesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profiles")
            .WithTags("Profiles")
            .WithOpenApi();

        // GET /api/profiles/user/{userId}
        group.MapGet("/user/{userId:guid}", async (
            Guid userId,
            IProfileService profileService,
            CancellationToken cancellationToken) =>
        {
            var profile = await profileService.GetProfileByUserIdAsync(userId, cancellationToken);
            return profile is not null ? Results.Ok(profile) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetProfileByUserId")
        .WithSummary("Get profile by user ID")
        .Produces<ProfileResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // GET /api/profiles/me - Get current user's profile
        group.MapGet("/me", async (
            IProfileService profileService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var profile = await profileService.GetProfileByUserIdAsync(userId, cancellationToken);
            return profile is not null ? Results.Ok(profile) : Results.NotFound(new { message = "Profile not found. Create one first." });
        })
        .RequireAuthorization()
        .WithName("GetMyProfile")
        .WithSummary("Get current user's profile")
        .Produces<ProfileResponseDto>(200)
        .Produces(401)
        .Produces(404)
        .WithOpenApi();

        // POST /api/profiles - Create or update profile
        group.MapPost("/", async (
            CreateProfileRequest request,
            IProfileService profileService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
                var profile = await profileService.CreateOrUpdateProfileAsync(userId, request, cancellationToken);
                return Results.Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization()
        .WithName("CreateOrUpdateProfile")
        .WithSummary("Create or update profile")
        .WithDescription("Creates a new profile or updates an existing one for the authenticated user")
        .Produces<ProfileResponseDto>(200)
        .Produces(400)
        .Produces(401)
        .WithOpenApi();

        // DELETE /api/profiles
        group.MapDelete("/", async (
            IProfileService profileService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var deleted = await profileService.DeleteProfileAsync(userId, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("DeleteProfile")
        .WithSummary("Delete profile")
        .WithDescription("Deletes the current user's profile")
        .Produces(204)
        .Produces(401)
        .Produces(404)
        .WithOpenApi();
    }
}

public static class BlogStatisticsEndpoints
{
    public static void MapBlogStatisticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/statistics")
            .WithTags("Statistics")
            .WithOpenApi();

        // GET /api/statistics
        group.MapGet("/", async (
            IBlogStatisticsService statisticsService,
            CancellationToken cancellationToken) =>
        {
            var stats = await statisticsService.GetStatisticsAsync(cancellationToken);
            return Results.Ok(stats);
        })
        .AllowAnonymous()
        .WithName("GetBlogStatistics")
        .WithSummary("Get blog statistics")
        .WithDescription("Returns comprehensive blog statistics including post counts, popular posts, and category stats")
        .Produces<BlogStatisticsDto>(200)
        .WithOpenApi();
    }
}
