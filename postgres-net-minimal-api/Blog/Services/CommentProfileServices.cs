using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.DTOs;
using postgres_net_minimal_api.Helpers;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Blog.Services;

// ==================== COMMENT SERVICE ====================

public interface ICommentService
{
    Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(
        Guid postId,
        bool onlyApproved = true,
        CancellationToken cancellationToken = default);

    Task<CommentResponseDto?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CommentResponseDto> CreateCommentAsync(
        CreateCommentRequest request,
        Guid authorId,
        CancellationToken cancellationToken = default);

    Task<CommentResponseDto?> UpdateCommentAsync(
        Guid id,
        UpdateCommentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteCommentAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ApproveCommentAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> RejectCommentAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<CommentResponseDto>> GetPendingCommentsAsync(CancellationToken cancellationToken = default);
}

public class CommentService(AppDbContext context) : ICommentService
{
    private readonly AppDbContext _context = context;

    public async Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(
        Guid postId,
        bool onlyApproved = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Comments
            .AsNoTracking()
            .Include(c => c.Author)
            .Include(c => c.Replies).ThenInclude(r => r.Author)
            .Where(c => c.PostId == postId && c.ParentId == null);

        if (onlyApproved)
        {
            query = query.Where(c => c.IsApproved);
        }

        var comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return comments.Select(c => c.ToDto()).ToList();
    }

    public async Task<CommentResponseDto?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .AsNoTracking()
            .Include(c => c.Author)
            .Include(c => c.Replies).ThenInclude(r => r.Author)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return comment?.ToDto();
    }

    public async Task<CommentResponseDto> CreateCommentAsync(
        CreateCommentRequest request,
        Guid authorId,
        CancellationToken cancellationToken = default)
    {
        // Validate post exists
        var postExists = await _context.Posts
            .AsNoTracking()
            .AnyAsync(p => p.Id == request.PostId, cancellationToken);

        if (!postExists)
        {
            throw new InvalidOperationException("The specified post does not exist");
        }

        // Validate parent comment if provided
        if (request.ParentId.HasValue)
        {
            var parentExists = await _context.Comments
                .AsNoTracking()
                .AnyAsync(c => c.Id == request.ParentId.Value && c.PostId == request.PostId, cancellationToken);

            if (!parentExists)
            {
                throw new InvalidOperationException("The specified parent comment does not exist");
            }
        }

        // Sanitize content
        var sanitizedContent = HtmlSanitizer.Sanitize(request.Content);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            AuthorId = authorId,
            ParentId = request.ParentId,
            Content = sanitizedContent,
            IsApproved = false, // Comments require approval by default
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);

        // NOTE: Comment count is NOT incremented here because comment requires approval
        // Comment count will be incremented when comment is approved via ApproveCommentAsync

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with author
        var createdComment = await _context.Comments
            .AsNoTracking()
            .Include(c => c.Author)
            .FirstAsync(c => c.Id == comment.Id, cancellationToken);

        return createdComment.ToDto();
    }

    public async Task<CommentResponseDto?> UpdateCommentAsync(
        Guid id,
        UpdateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (comment is null)
        {
            return null;
        }

        // Sanitize content
        var sanitizedContent = HtmlSanitizer.Sanitize(request.Content);

        comment.Content = sanitizedContent;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return comment.ToDto();
    }

    public async Task<bool> DeleteCommentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (comment is null)
        {
            return false;
        }

        _context.Comments.Remove(comment);

        // Only decrement count if comment was approved (visible)
        if (comment.IsApproved && comment.Post is not null && comment.Post.CommentCount > 0)
        {
            comment.Post.CommentCount--;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ApproveCommentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (comment is null)
        {
            return false;
        }

        // Only increment count if transitioning from not approved to approved
        if (!comment.IsApproved)
        {
            comment.IsApproved = true;

            // Increment post comment count when comment is approved
            if (comment.Post is not null)
            {
                comment.Post.CommentCount++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RejectCommentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (comment is null)
        {
            return false;
        }

        // Only decrement count if transitioning from approved to not approved
        if (comment.IsApproved)
        {
            comment.IsApproved = false;

            // Decrement post comment count when comment is rejected
            if (comment.Post is not null && comment.Post.CommentCount > 0)
            {
                comment.Post.CommentCount--;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<List<CommentResponseDto>> GetPendingCommentsAsync(CancellationToken cancellationToken = default)
    {
        var comments = await _context.Comments
            .AsNoTracking()
            .Include(c => c.Author)
            .Include(c => c.Post)
            .Where(c => !c.IsApproved)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return comments.Select(c => c.ToDto()).ToList();
    }
}

// ==================== PROFILE SERVICE ====================

public interface IProfileService
{
    Task<ProfileResponseDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ProfileResponseDto> CreateOrUpdateProfileAsync(Guid userId, CreateProfileRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class ProfileService(AppDbContext context) : IProfileService
{
    private readonly AppDbContext _context = context;

    public async Task<ProfileResponseDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        return profile?.ToDto();
    }

    public async Task<ProfileResponseDto> CreateOrUpdateProfileAsync(
        Guid userId,
        CreateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validate user exists
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException("User not found");
        }

        var existingProfile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (existingProfile is not null)
        {
            // Update existing profile
            existingProfile.Bio = request.Bio;
            existingProfile.AvatarUrl = request.AvatarUrl;
            existingProfile.WebsiteUrl = request.WebsiteUrl;
            existingProfile.TwitterHandle = request.TwitterHandle;
            existingProfile.GitHubHandle = request.GitHubHandle;
            existingProfile.LinkedInHandle = request.LinkedInHandle;
            existingProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return existingProfile.ToDto();
        }
        else
        {
            // Create new profile
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Bio = request.Bio,
                AvatarUrl = request.AvatarUrl,
                WebsiteUrl = request.WebsiteUrl,
                TwitterHandle = request.TwitterHandle,
                GitHubHandle = request.GitHubHandle,
                LinkedInHandle = request.LinkedInHandle,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);

            return profile.ToDto();
        }
    }

    public async Task<bool> DeleteProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile is null)
        {
            return false;
        }

        _context.Profiles.Remove(profile);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
