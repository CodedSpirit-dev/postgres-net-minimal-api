using System.Security.Claims;
using postgres_net_minimal_api.Blog.Services;
using postgres_net_minimal_api.Blog.DTOs;
using postgres_net_minimal_api.Authorization.Services;
using postgres_net_minimal_api.Authorization.Enums;
using postgres_net_minimal_api.Helpers;

namespace postgres_net_minimal_api.Blog.Controllers;

public static class PostsEndpoints
{
    public static void MapPostsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/posts")
            .WithTags("Posts")
            .WithOpenApi();

        // GET /api/posts - Get all posts with pagination
        group.MapGet("/", async (
            IPostService postService,
            int page = 1,
            int pageSize = 20,
            bool? isPublished = true,
            CancellationToken cancellationToken = default) =>
        {
            var result = await postService.GetAllPostsAsync(page, pageSize, isPublished, cancellationToken);
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .WithName("GetAllPosts")
        .WithSummary("Get all blog posts")
        .WithDescription("Returns a paginated list of blog posts. Can filter by published status.")
        .Produces<PagedResult<PostSummaryDto>>(200)
        .WithOpenApi();

        // POST /api/posts/search - Advanced search
        group.MapPost("/search", async (
            PostSearchRequest request,
            IPostService postService,
            CancellationToken cancellationToken) =>
        {
            var result = await postService.SearchPostsAsync(request, cancellationToken);
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .WithName("SearchPosts")
        .WithSummary("Search blog posts")
        .WithDescription("Advanced search with filters by category, tags, author, date range, and search query")
        .Produces<PagedResult<PostSummaryDto>>(200)
        .WithOpenApi();

        // GET /api/posts/{id} - Get post by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            IPostService postService,
            CancellationToken cancellationToken) =>
        {
            var post = await postService.GetPostByIdAsync(id, cancellationToken);

            if (post is null)
            {
                return Results.NotFound();
            }

            // Increment view count - await to ensure it completes
            await postService.IncrementViewCountAsync(id, cancellationToken);

            return Results.Ok(post);
        })
        .AllowAnonymous()
        .WithName("GetPostById")
        .WithSummary("Get post by ID")
        .WithDescription("Returns a single post by its unique identifier. Increments view count.")
        .Produces<PostResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // GET /api/posts/slug/{slug} - Get post by slug
        group.MapGet("/slug/{slug}", async (
            string slug,
            IPostService postService,
            CancellationToken cancellationToken) =>
        {
            var post = await postService.GetPostBySlugAsync(slug, cancellationToken);

            if (post is null)
            {
                return Results.NotFound();
            }

            // Increment view count - await to ensure it completes
            await postService.IncrementViewCountAsync(post.Id, cancellationToken);

            return Results.Ok(post);
        })
        .AllowAnonymous()
        .WithName("GetPostBySlug")
        .WithSummary("Get post by slug")
        .WithDescription("Returns a single post by its URL-friendly slug. Increments view count.")
        .Produces<PostResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // POST /api/posts - Create new post
        group.MapPost("/", async (
            CreatePostRequest request,
            IPostService postService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check if user has permission to create posts
            var hasPermission = await permissionChecker.HasPermissionAsync(userId, ResourceType.Posts, ActionType.Create);
            if (!hasPermission)
            {
                return Results.Forbid();
            }

            try
            {
                var post = await postService.CreatePostAsync(request, userId, cancellationToken);
                return Results.Created($"/api/posts/{post.Id}", post);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization()
        .WithName("CreatePost")
        .WithSummary("Create new post")
        .WithDescription("Creates a new blog post. Requires Posts.Create permission. Auto-generates slug and excerpt if not provided.")
        .Produces<PostResponseDto>(201)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .WithOpenApi();

        // PUT /api/posts/{id} - Update post
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePostRequest request,
            IPostService postService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check permission AND ownership (user can edit their own posts OR has Posts.Manage)
            var canEdit = await permissionChecker.HasPermissionAndOwnsResourceAsync(
                userId, ResourceType.Posts, ActionType.Edit, id, cancellationToken);

            if (!canEdit)
            {
                return Results.Forbid();
            }

            try
            {
                var post = await postService.UpdatePostAsync(id, request, cancellationToken);

                if (post is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(post);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization()
        .WithName("UpdatePost")
        .WithSummary("Update post")
        .WithDescription("Updates an existing blog post. Requires Posts.Edit permission and ownership of the post, OR Posts.Manage permission.")
        .Produces<PostResponseDto>(200)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // DELETE /api/posts/{id} - Delete post
        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPostService postService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check permission AND ownership (user can delete their own posts OR has Posts.Manage)
            var canDelete = await permissionChecker.HasPermissionAndOwnsResourceAsync(
                userId, ResourceType.Posts, ActionType.Delete, id, cancellationToken);

            if (!canDelete)
            {
                return Results.Forbid();
            }

            var deleted = await postService.DeletePostAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("DeletePost")
        .WithSummary("Delete post")
        .WithDescription("Permanently deletes a blog post. Requires Posts.Delete permission and ownership of the post, OR Posts.Manage permission.")
        .Produces(204)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // PATCH /api/posts/{id}/publish - Publish a post
        group.MapPatch("/{id:guid}/publish", async (
            Guid id,
            IPostService postService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check permission AND ownership (user can publish their own posts OR has Posts.Manage)
            var canPublish = await permissionChecker.HasPermissionAndOwnsResourceAsync(
                userId, ResourceType.Posts, ActionType.Publish, id, cancellationToken);

            if (!canPublish)
            {
                return Results.Forbid();
            }

            var post = await postService.PublishPostAsync(id, cancellationToken);
            if (post is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(post);
        })
        .RequireAuthorization()
        .WithName("PublishPost")
        .WithSummary("Publish a post")
        .WithDescription("Publishes a post making it visible to the public. Requires Posts.Publish permission and ownership, OR Posts.Manage permission.")
        .Produces<PostResponseDto>(200)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // PATCH /api/posts/{id}/unpublish - Unpublish a post
        group.MapPatch("/{id:guid}/unpublish", async (
            Guid id,
            IPostService postService,
            IPermissionChecker permissionChecker,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // Check permission AND ownership (user can unpublish their own posts OR has Posts.Manage)
            var canUnpublish = await permissionChecker.HasPermissionAndOwnsResourceAsync(
                userId, ResourceType.Posts, ActionType.Unpublish, id, cancellationToken);

            if (!canUnpublish)
            {
                return Results.Forbid();
            }

            var post = await postService.UnpublishPostAsync(id, cancellationToken);
            if (post is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(post);
        })
        .RequireAuthorization()
        .WithName("UnpublishPost")
        .WithSummary("Unpublish a post")
        .WithDescription("Unpublishes a post making it a draft again. Requires Posts.Unpublish permission and ownership, OR Posts.Manage permission.")
        .Produces<PostResponseDto>(200)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // GET /api/posts/popular/most-viewed - Get most viewed posts
        group.MapGet("/popular/most-viewed", async (
            IPostService postService,
            int count = 10,
            CancellationToken cancellationToken = default) =>
        {
            var posts = await postService.GetMostViewedPostsAsync(count, cancellationToken);
            return Results.Ok(posts);
        })
        .AllowAnonymous()
        .WithName("GetMostViewedPosts")
        .WithSummary("Get most viewed posts")
        .WithDescription("Returns the most viewed published posts")
        .Produces<List<PopularPostDto>>(200)
        .WithOpenApi();

        // GET /api/posts/popular/most-commented - Get most commented posts
        group.MapGet("/popular/most-commented", async (
            IPostService postService,
            int count = 10,
            CancellationToken cancellationToken = default) =>
        {
            var posts = await postService.GetMostCommentedPostsAsync(count, cancellationToken);
            return Results.Ok(posts);
        })
        .AllowAnonymous()
        .WithName("GetMostCommentedPosts")
        .WithSummary("Get most commented posts")
        .WithDescription("Returns the most commented published posts")
        .Produces<List<PopularPostDto>>(200)
        .WithOpenApi();
    }
}
