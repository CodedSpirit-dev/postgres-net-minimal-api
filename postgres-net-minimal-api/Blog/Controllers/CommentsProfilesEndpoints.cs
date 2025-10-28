using System.Security.Claims;
using postgres_net_minimal_api.Blog.Services;
using postgres_net_minimal_api.Blog.DTOs;
using postgres_net_minimal_api.Authorization.Services;
using postgres_net_minimal_api.Authorization.Enums;

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

        // GET /api/comments/pending - Get pending comments (requires Moderate permission)
        group.MapGet("/pending", async (
            ICommentService commentService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check if user has permission to moderate comments
            var hasPermission = await permissionChecker.HasPermissionAsync(userId, ResourceType.Comments, ActionType.Moderate);
            if (!hasPermission)
            {
                return Results.Forbid();
            }

            var comments = await commentService.GetPendingCommentsAsync(cancellationToken);
            return Results.Ok(comments);
        })
        .RequireAuthorization()
        .WithName("GetPendingComments")
        .WithSummary("Get pending comments")
        .WithDescription("Returns all comments awaiting approval. Requires Comments.Moderate permission.")
        .Produces<List<CommentResponseDto>>(200)
        .Produces(401)
        .Produces(403)
        .WithOpenApi();

        // POST /api/comments
        group.MapPost("/", async (
            CreateCommentRequest request,
            ICommentService commentService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check if user has permission to create comments
            var hasPermission = await permissionChecker.HasPermissionAsync(userId, ResourceType.Comments, ActionType.Create);
            if (!hasPermission)
            {
                return Results.Forbid();
            }

            try
            {
                var comment = await commentService.CreateCommentAsync(request, userId, cancellationToken);
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
        .WithDescription("Creates a new comment on a post. Requires Comments.Create permission. Comments require approval.")
        .Produces<CommentResponseDto>(201)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .WithOpenApi();

        // PUT /api/comments/{id}
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCommentRequest request,
            ICommentService commentService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check permission AND ownership (user can edit their own comments OR has Comments.Manage)
            var canEdit = await permissionChecker.HasPermissionAndOwnsResourceAsync(
                userId, ResourceType.Comments, ActionType.Edit, id, cancellationToken);

            if (!canEdit)
            {
                return Results.Forbid();
            }

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
        .WithDescription("Updates a comment. Requires Comments.Edit permission and ownership, OR Comments.Manage permission.")
        .Produces<CommentResponseDto>(200)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // DELETE /api/comments/{id}
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICommentService commentService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check permission AND ownership (user can delete their own comments OR has Comments.Manage)
            var canDelete = await permissionChecker.HasPermissionAndOwnsResourceAsync(
                userId, ResourceType.Comments, ActionType.Delete, id, cancellationToken);

            if (!canDelete)
            {
                return Results.Forbid();
            }

            var deleted = await commentService.DeleteCommentAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("DeleteComment")
        .WithSummary("Delete comment")
        .WithDescription("Permanently deletes a comment. Requires Comments.Delete permission and ownership, OR Comments.Manage permission.")
        .Produces(204)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // POST /api/comments/{id}/approve
        group.MapPost("/{id:guid}/approve", async (
            Guid id,
            ICommentService commentService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check if user has permission to approve comments
            var hasPermission = await permissionChecker.HasPermissionAsync(userId, ResourceType.Comments, ActionType.Approve);
            if (!hasPermission)
            {
                return Results.Forbid();
            }

            var approved = await commentService.ApproveCommentAsync(id, cancellationToken);
            return approved ? Results.Ok(new { message = "Comment approved" }) : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("ApproveComment")
        .WithSummary("Approve comment")
        .WithDescription("Approves a pending comment. Requires Comments.Approve permission.")
        .Produces(200)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // POST /api/comments/{id}/reject
        group.MapPost("/{id:guid}/reject", async (
            Guid id,
            ICommentService commentService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check if user has permission to reject comments
            var hasPermission = await permissionChecker.HasPermissionAsync(userId, ResourceType.Comments, ActionType.Reject);
            if (!hasPermission)
            {
                return Results.Forbid();
            }

            var rejected = await commentService.RejectCommentAsync(id, cancellationToken);
            return rejected ? Results.Ok(new { message = "Comment rejected" }) : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("RejectComment")
        .WithSummary("Reject comment")
        .WithDescription("Rejects an approved comment. Requires Comments.Reject permission.")
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
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");

            // Allow viewing own profile or require Profiles.View permission for other profiles
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var currentUserId))
            {
                if (currentUserId != userId)
                {
                    var hasPermission = await permissionChecker.HasPermissionAsync(
                        currentUserId, ResourceType.Profiles, ActionType.View);

                    if (!hasPermission)
                    {
                        return Results.Forbid();
                    }
                }
            }
            else
            {
                // Anonymous users cannot view profiles
                return Results.Unauthorized();
            }

            var profile = await profileService.GetProfileByUserIdAsync(userId, cancellationToken);
            return profile is not null ? Results.Ok(profile) : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("GetProfileByUserId")
        .WithSummary("Get profile by user ID")
        .WithDescription("Returns a user's profile. Users can view their own profile or require Profiles.View permission to view others.")
        .Produces<ProfileResponseDto>(200)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // GET /api/profiles/me - Get current user's profile
        group.MapGet("/me", async (
            IProfileService profileService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

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
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
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
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

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
