using postgres_net_minimal_api.Blog.Services;
using postgres_net_minimal_api.Blog.DTOs;

namespace postgres_net_minimal_api.Blog.Controllers;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories")
            .WithOpenApi();

        // GET /api/categories
        group.MapGet("/", async (
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var categories = await categoryService.GetAllCategoriesAsync(cancellationToken);
            return Results.Ok(categories);
        })
        .AllowAnonymous()
        .WithName("GetAllCategories")
        .WithSummary("Get all categories")
        .WithDescription("Returns all blog categories with post counts")
        .Produces<List<CategoryResponseDto>>(200)
        .WithOpenApi();

        // GET /api/categories/{id}
        group.MapGet("/{id:guid}", async (
            Guid id,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var category = await categoryService.GetCategoryByIdAsync(id, cancellationToken);
            return category is not null ? Results.Ok(category) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetCategoryById")
        .WithSummary("Get category by ID")
        .Produces<CategoryResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // GET /api/categories/slug/{slug}
        group.MapGet("/slug/{slug}", async (
            string slug,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var category = await categoryService.GetCategoryBySlugAsync(slug, cancellationToken);
            return category is not null ? Results.Ok(category) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetCategoryBySlug")
        .WithSummary("Get category by slug")
        .Produces<CategoryResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // POST /api/categories
        group.MapPost("/", async (
            CreateCategoryRequest request,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var category = await categoryService.CreateCategoryAsync(request, cancellationToken);
                return Results.Created($"/api/categories/{category.Id}", category);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("CreateCategory")
        .WithSummary("Create new category")
        .WithDescription("Creates a new blog category. Requires Admin role.")
        .Produces<CategoryResponseDto>(201)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .WithOpenApi();

        // PUT /api/categories/{id}
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCategoryRequest request,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var category = await categoryService.UpdateCategoryAsync(id, request, cancellationToken);
                return category is not null ? Results.Ok(category) : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("UpdateCategory")
        .WithSummary("Update category")
        .Produces<CategoryResponseDto>(200)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();

        // DELETE /api/categories/{id}
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var deleted = await categoryService.DeleteCategoryAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("DeleteCategory")
        .WithSummary("Delete category")
        .Produces(204)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();
    }
}

public static class TagsEndpoints
{
    public static void MapTagsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tags")
            .WithTags("Tags")
            .WithOpenApi();

        // GET /api/tags
        group.MapGet("/", async (
            ITagService tagService,
            CancellationToken cancellationToken) =>
        {
            var tags = await tagService.GetAllTagsAsync(cancellationToken);
            return Results.Ok(tags);
        })
        .AllowAnonymous()
        .WithName("GetAllTags")
        .WithSummary("Get all tags")
        .WithDescription("Returns all blog tags with post counts")
        .Produces<List<TagResponseDto>>(200)
        .WithOpenApi();

        // GET /api/tags/{id}
        group.MapGet("/{id:guid}", async (
            Guid id,
            ITagService tagService,
            CancellationToken cancellationToken) =>
        {
            var tag = await tagService.GetTagByIdAsync(id, cancellationToken);
            return tag is not null ? Results.Ok(tag) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetTagById")
        .WithSummary("Get tag by ID")
        .Produces<TagResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // GET /api/tags/slug/{slug}
        group.MapGet("/slug/{slug}", async (
            string slug,
            ITagService tagService,
            CancellationToken cancellationToken) =>
        {
            var tag = await tagService.GetTagBySlugAsync(slug, cancellationToken);
            return tag is not null ? Results.Ok(tag) : Results.NotFound();
        })
        .AllowAnonymous()
        .WithName("GetTagBySlug")
        .WithSummary("Get tag by slug")
        .Produces<TagResponseDto>(200)
        .Produces(404)
        .WithOpenApi();

        // POST /api/tags
        group.MapPost("/", async (
            CreateTagRequest request,
            ITagService tagService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var tag = await tagService.CreateTagAsync(request, cancellationToken);
                return Results.Created($"/api/tags/{tag.Id}", tag);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("CreateTag")
        .WithSummary("Create new tag")
        .WithDescription("Creates a new blog tag. Requires Admin role.")
        .Produces<TagResponseDto>(201)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .WithOpenApi();

        // DELETE /api/tags/{id}
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ITagService tagService,
            CancellationToken cancellationToken) =>
        {
            var deleted = await tagService.DeleteTagAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("DeleteTag")
        .WithSummary("Delete tag")
        .Produces(204)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .WithOpenApi();
    }
}
