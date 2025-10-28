using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Blog.DTOs;
using postgres_net_minimal_api.Helpers;
using postgres_net_minimal_api.Blog.Models;

namespace postgres_net_minimal_api.Blog.Services;

// ==================== CATEGORY SERVICE ====================

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}

public class CategoryService(AppDbContext context) : ICategoryService
{
    private readonly AppDbContext _context = context;

    public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponseDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.Posts.Count(p => p.IsPublished),
                c.CreatedAt,
                c.UpdatedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryResponseDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.Posts.Count(p => p.IsPublished),
                c.CreatedAt,
                c.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryResponseDto?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Slug == slug)
            .Select(c => new CategoryResponseDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.Posts.Count(p => p.IsPublished),
                c.CreatedAt,
                c.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        // Generate slug
        var baseSlug = SlugHelper.GenerateSlug(request.Name);
        var slug = await SlugHelper.GenerateUniqueSlugAsync(
            baseSlug,
            async (s, excludeId) => await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Slug == s && (excludeId == null || c.Id != excludeId), cancellationToken),
            null
        );

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.ToDto(0);
    }

    public async Task<CategoryResponseDto?> UpdateCategoryAsync(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync([id], cancellationToken);

        if (category is null)
        {
            return null;
        }

        // Update slug if name changed
        if (category.Name != request.Name)
        {
            var baseSlug = SlugHelper.GenerateSlug(request.Name);
            category.Slug = await SlugHelper.GenerateUniqueSlugAsync(
                baseSlug,
                async (s, excludeId) => await _context.Categories
                    .AsNoTracking()
                    .AnyAsync(c => c.Slug == s && c.Id != id, cancellationToken),
                id
            );
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var postCount = await _context.Posts
            .AsNoTracking()
            .CountAsync(p => p.CategoryId == id && p.IsPublished, cancellationToken);

        return category.ToDto(postCount);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync([id], cancellationToken);

        if (category is null)
        {
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

// ==================== TAG SERVICE ====================

public interface ITagService
{
    Task<List<TagResponseDto>> GetAllTagsAsync(CancellationToken cancellationToken = default);
    Task<TagResponseDto?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TagResponseDto?> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<TagResponseDto> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTagAsync(Guid id, CancellationToken cancellationToken = default);
}

public class TagService(AppDbContext context) : ITagService
{
    private readonly AppDbContext _context = context;

    public async Task<List<TagResponseDto>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TagResponseDto(
                t.Id,
                t.Name,
                t.Slug,
                t.PostTags.Count(pt => pt.Post.IsPublished),
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<TagResponseDto?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TagResponseDto(
                t.Id,
                t.Name,
                t.Slug,
                t.PostTags.Count(pt => pt.Post.IsPublished),
                t.CreatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TagResponseDto?> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .AsNoTracking()
            .Where(t => t.Slug == slug)
            .Select(t => new TagResponseDto(
                t.Id,
                t.Name,
                t.Slug,
                t.PostTags.Count(pt => pt.Post.IsPublished),
                t.CreatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TagResponseDto> CreateTagAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken = default)
    {
        // Check for duplicate name
        var nameExists = await _context.Tags
            .AsNoTracking()
            .AnyAsync(t => t.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException("A tag with this name already exists");
        }

        // Generate slug
        var baseSlug = SlugHelper.GenerateSlug(request.Name);
        var slug = await SlugHelper.GenerateUniqueSlugAsync(
            baseSlug,
            async (s, excludeId) => await _context.Tags
                .AsNoTracking()
                .AnyAsync(t => t.Slug == s && (excludeId == null || t.Id != excludeId), cancellationToken),
            null
        );

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = slug,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return tag.ToDto(0);
    }

    public async Task<bool> DeleteTagAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags.FindAsync([id], cancellationToken);

        if (tag is null)
        {
            return false;
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
