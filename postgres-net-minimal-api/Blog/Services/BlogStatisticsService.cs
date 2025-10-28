using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Blog.DTOs;

namespace postgres_net_minimal_api.Blog.Services;

public interface IBlogStatisticsService
{
    Task<BlogStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

public class BlogStatisticsService(AppDbContext context) : IBlogStatisticsService
{
    private readonly AppDbContext _context = context;

    public async Task<BlogStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        // Run queries in parallel for better performance
        var totalPostsTask = _context.Posts.AsNoTracking().CountAsync(cancellationToken);
        var publishedPostsTask = _context.Posts.AsNoTracking().CountAsync(p => p.IsPublished, cancellationToken);
        var draftPostsTask = _context.Posts.AsNoTracking().CountAsync(p => !p.IsPublished, cancellationToken);
        var totalCommentsTask = _context.Comments.AsNoTracking().CountAsync(cancellationToken);
        var pendingCommentsTask = _context.Comments.AsNoTracking().CountAsync(c => !c.IsApproved, cancellationToken);
        var totalCategoriesTask = _context.Categories.AsNoTracking().CountAsync(cancellationToken);
        var totalTagsTask = _context.Tags.AsNoTracking().CountAsync(cancellationToken);

        var mostViewedPostsTask = _context.Posts
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.ViewCount)
            .Take(10)
            .Select(p => new PopularPostDto(
                p.Id,
                p.Title,
                p.Slug,
                p.ViewCount,
                p.CommentCount,
                p.PublishedAt
            ))
            .ToListAsync(cancellationToken);

        var mostCommentedPostsTask = _context.Posts
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.CommentCount)
            .Take(10)
            .Select(p => new PopularPostDto(
                p.Id,
                p.Title,
                p.Slug,
                p.ViewCount,
                p.CommentCount,
                p.PublishedAt
            ))
            .ToListAsync(cancellationToken);

        var categoryStatsTask = _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryStatsDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Posts.Count(p => p.IsPublished)
            ))
            .OrderByDescending(c => c.PostCount)
            .ToListAsync(cancellationToken);

        // Wait for all tasks to complete
        await Task.WhenAll(
            totalPostsTask,
            publishedPostsTask,
            draftPostsTask,
            totalCommentsTask,
            pendingCommentsTask,
            totalCategoriesTask,
            totalTagsTask,
            mostViewedPostsTask,
            mostCommentedPostsTask,
            categoryStatsTask
        );

        return new BlogStatisticsDto(
            await totalPostsTask,
            await publishedPostsTask,
            await draftPostsTask,
            await totalCommentsTask,
            await pendingCommentsTask,
            await totalCategoriesTask,
            await totalTagsTask,
            await mostViewedPostsTask,
            await mostCommentedPostsTask,
            await categoryStatsTask
        );
    }
}
