using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.DTOs;
using postgres_net_minimal_api.Helpers;
using postgres_net_minimal_api.Models;
using postgres_net_minimal_api.Services;

namespace postgres_net_minimal_api.Blog.Services;

public class PostService(AppDbContext context) : IPostService
{
    private readonly AppDbContext _context = context;

    public async Task<PagedResult<PostSummaryDto>> GetAllPostsAsync(
        int page,
        int pageSize,
        bool? isPublished = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .AsQueryable();

        if (isPublished.HasValue)
        {
            query = query.Where(p => p.IsPublished == isPublished.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var posts = await query
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var postDtos = posts.Select(p => p.ToSummaryDto()).ToList();

        return new PagedResult<PostSummaryDto>(
            postDtos,
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }

    public async Task<PagedResult<PostSummaryDto>> SearchPostsAsync(
        PostSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var query = _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .AsQueryable();

        // Filter by published status
        if (request.IsPublished.HasValue)
        {
            query = query.Where(p => p.IsPublished == request.IsPublished.Value);
        }

        // Filter by search query
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var searchTerms = SearchNormalizer.GetSearchTerms(request.Query);
            foreach (var term in searchTerms)
            {
                query = query.Where(p =>
                    p.Title.ToLower().Contains(term) ||
                    p.Content.ToLower().Contains(term) ||
                    (p.Excerpt != null && p.Excerpt.ToLower().Contains(term)));
            }
        }

        // Filter by category
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        // Filter by tags
        if (request.TagIds.Count > 0)
        {
            query = query.Where(p => p.PostTags.Any(pt => request.TagIds.Contains(pt.TagId)));
        }

        // Filter by author
        if (request.AuthorId.HasValue)
        {
            query = query.Where(p => p.AuthorId == request.AuthorId.Value);
        }

        // Filter by date range
        if (request.PublishedAfter.HasValue)
        {
            query = query.Where(p => p.PublishedAt >= request.PublishedAfter.Value);
        }

        if (request.PublishedBefore.HasValue)
        {
            query = query.Where(p => p.PublishedAt <= request.PublishedBefore.Value);
        }

        // Order by
        query = request.OrderBy?.ToLower() switch
        {
            "title" => request.Descending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),
            "viewcount" => request.Descending
                ? query.OrderByDescending(p => p.ViewCount)
                : query.OrderBy(p => p.ViewCount),
            "commentcount" => request.Descending
                ? query.OrderByDescending(p => p.CommentCount)
                : query.OrderBy(p => p.CommentCount),
            _ => request.Descending
                ? query.OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                : query.OrderBy(p => p.PublishedAt ?? p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var postDtos = posts.Select(p => p.ToSummaryDto()).ToList();

        return new PagedResult<PostSummaryDto>(
            postDtos,
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }

    public async Task<PostResponseDto?> GetPostByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return post?.ToDto();
    }

    public async Task<PostResponseDto?> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

        return post?.ToDto();
    }

    public async Task<PostResponseDto> CreatePostAsync(
        CreatePostRequest request,
        Guid authorId,
        CancellationToken cancellationToken = default)
    {
        // Validate category if provided
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == request.CategoryId.Value, cancellationToken);

            if (!categoryExists)
            {
                throw new InvalidOperationException("The specified category does not exist");
            }
        }

        // Generate slug
        var baseSlug = SlugHelper.GenerateSlug(request.Title);
        var slug = await SlugHelper.GenerateUniqueSlugAsync(
            baseSlug,
            async (s, excludeId) => await _context.Posts
                .AsNoTracking()
                .AnyAsync(p => p.Slug == s && (excludeId == null || p.Id != excludeId), cancellationToken),
            null
        );

        // Sanitize HTML content
        var sanitizedContent = HtmlSanitizer.Sanitize(request.Content);

        // Generate excerpt if not provided
        var excerpt = string.IsNullOrWhiteSpace(request.Excerpt)
            ? HtmlSanitizer.GenerateExcerpt(sanitizedContent, 160)
            : request.Excerpt;

        // Create post
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = sanitizedContent,
            Slug = slug,
            Excerpt = excerpt,
            FeaturedImageUrl = request.FeaturedImageUrl,
            MetaTitle = request.MetaTitle ?? request.Title,
            MetaDescription = request.MetaDescription ?? excerpt,
            AuthorId = authorId,
            CategoryId = request.CategoryId,
            IsPublished = request.IsPublished,
            PublishedAt = request.IsPublished
                ? (request.PublishedAt ?? DateTime.UtcNow)
                : null,
            ViewCount = 0,
            CommentCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);

        // Add tags
        if (request.TagIds.Count > 0)
        {
            var validTagIds = await _context.Tags
                .AsNoTracking()
                .Where(t => request.TagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            foreach (var tagId in validTagIds)
            {
                _context.PostTags.Add(new PostTag
                {
                    PostId = post.Id,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with related data
        var createdPost = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstAsync(p => p.Id == post.Id, cancellationToken);

        return createdPost.ToDto();
    }

    public async Task<PostResponseDto?> UpdatePostAsync(
        Guid id,
        UpdatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (post is null)
        {
            return null;
        }

        // Validate category if provided
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == request.CategoryId.Value, cancellationToken);

            if (!categoryExists)
            {
                throw new InvalidOperationException("The specified category does not exist");
            }
        }

        // Update slug if title changed
        if (post.Title != request.Title)
        {
            var baseSlug = SlugHelper.GenerateSlug(request.Title);
            post.Slug = await SlugHelper.GenerateUniqueSlugAsync(
                baseSlug,
                async (s, excludeId) => await _context.Posts
                    .AsNoTracking()
                    .AnyAsync(p => p.Slug == s && p.Id != id, cancellationToken),
                id
            );
        }

        // Sanitize HTML content
        var sanitizedContent = HtmlSanitizer.Sanitize(request.Content);

        // Generate excerpt if not provided
        var excerpt = string.IsNullOrWhiteSpace(request.Excerpt)
            ? HtmlSanitizer.GenerateExcerpt(sanitizedContent, 160)
            : request.Excerpt;

        // Update post properties
        post.Title = request.Title;
        post.Content = sanitizedContent;
        post.Excerpt = excerpt;
        post.FeaturedImageUrl = request.FeaturedImageUrl;
        post.MetaTitle = request.MetaTitle ?? request.Title;
        post.MetaDescription = request.MetaDescription ?? excerpt;
        post.CategoryId = request.CategoryId;
        post.IsPublished = request.IsPublished;
        post.PublishedAt = request.IsPublished
            ? (request.PublishedAt ?? post.PublishedAt ?? DateTime.UtcNow)
            : null;
        post.UpdatedAt = DateTime.UtcNow;

        // Update tags
        // Remove existing tags
        _context.PostTags.RemoveRange(post.PostTags);

        // Add new tags
        if (request.TagIds.Count > 0)
        {
            var validTagIds = await _context.Tags
                .AsNoTracking()
                .Where(t => request.TagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            foreach (var tagId in validTagIds)
            {
                _context.PostTags.Add(new PostTag
                {
                    PostId = post.Id,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with related data
        var updatedPost = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstAsync(p => p.Id == id, cancellationToken);

        return updatedPost.ToDto();
    }

    public async Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.FindAsync([id], cancellationToken);

        if (post is null)
        {
            return false;
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<PostResponseDto?> PublishPostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.FindAsync([id], cancellationToken);

        if (post is null)
        {
            return null;
        }

        // Validate post can be published
        if (string.IsNullOrWhiteSpace(post.Content) || post.Content.Length < 50)
        {
            throw new InvalidOperationException("Post must have at least 50 characters of content to be published");
        }

        if (string.IsNullOrWhiteSpace(post.Title) || post.Title.Length < 5)
        {
            throw new InvalidOperationException("Post must have a title with at least 5 characters to be published");
        }

        // If already published, return current state (idempotent operation)
        if (post.IsPublished)
        {
            var currentPost = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            return currentPost?.ToDto();
        }

        post.IsPublished = true;
        post.PublishedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Retrieve the updated post with all related data
        var updatedPost = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return updatedPost?.ToDto();
    }

    public async Task<PostResponseDto?> UnpublishPostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.FindAsync([id], cancellationToken);

        if (post is null)
        {
            return null;
        }

        post.IsPublished = false;
        post.PublishedAt = null;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Retrieve the updated post with all related data
        var updatedPost = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return updatedPost?.ToDto();
    }

    public async Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.FindAsync([id], cancellationToken);

        if (post is not null)
        {
            post.ViewCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<PopularPostDto>> GetMostViewedPostsAsync(
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.ViewCount)
            .Take(count)
            .Select(p => new PopularPostDto(
                p.Id,
                p.Title,
                p.Slug,
                p.ViewCount,
                p.CommentCount,
                p.PublishedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PopularPostDto>> GetMostCommentedPostsAsync(
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.CommentCount)
            .Take(count)
            .Select(p => new PopularPostDto(
                p.Id,
                p.Title,
                p.Slug,
                p.ViewCount,
                p.CommentCount,
                p.PublishedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
