using System.ComponentModel.DataAnnotations;
using postgres_net_minimal_api.Models;
using postgres_net_minimal_api.Services;

namespace postgres_net_minimal_api.DTOs;

// ==================== PROFILE DTOs ====================

public record CreateProfileRequest
{
    [MaxLength(1000)]
    public string? Bio { get; init; }

    [MaxLength(500)]
    [Url]
    public string? AvatarUrl { get; init; }

    [MaxLength(500)]
    [Url]
    public string? WebsiteUrl { get; init; }

    [MaxLength(100)]
    public string? TwitterHandle { get; init; }

    [MaxLength(100)]
    public string? GitHubHandle { get; init; }

    [MaxLength(100)]
    public string? LinkedInHandle { get; init; }
}

public record UpdateProfileRequest
{
    [MaxLength(1000)]
    public string? Bio { get; init; }

    [MaxLength(500)]
    [Url]
    public string? AvatarUrl { get; init; }

    [MaxLength(500)]
    [Url]
    public string? WebsiteUrl { get; init; }

    [MaxLength(100)]
    public string? TwitterHandle { get; init; }

    [MaxLength(100)]
    public string? GitHubHandle { get; init; }

    [MaxLength(100)]
    public string? LinkedInHandle { get; init; }
}

public record ProfileResponseDto(
    Guid Id,
    Guid UserId,
    string? Bio,
    string? AvatarUrl,
    string? WebsiteUrl,
    string? TwitterHandle,
    string? GitHubHandle,
    string? LinkedInHandle,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// ==================== CATEGORY DTOs ====================

public record CreateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; init; }

    [MaxLength(500)]
    public string? Description { get; init; }
}

public record UpdateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; init; }

    [MaxLength(500)]
    public string? Description { get; init; }
}

public record CategoryResponseDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    int PostCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// ==================== TAG DTOs ====================

public record CreateTagRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public required string Name { get; init; }
}

public record TagResponseDto(
    Guid Id,
    string Name,
    string Slug,
    int PostCount,
    DateTime CreatedAt
);

// ==================== POST DTOs ====================

public record CreatePostRequest
{
    [Required]
    [StringLength(300, MinimumLength = 5)]
    public required string Title { get; init; }

    [Required]
    [MinLength(50)]
    public required string Content { get; init; }

    [MaxLength(500)]
    public string? Excerpt { get; init; }

    [MaxLength(500)]
    [Url]
    public string? FeaturedImageUrl { get; init; }

    public Guid? CategoryId { get; init; }

    public List<Guid> TagIds { get; init; } = [];

    // SEO
    [MaxLength(70)]
    public string? MetaTitle { get; init; }

    [MaxLength(160)]
    public string? MetaDescription { get; init; }

    public bool IsPublished { get; init; }
    public DateTime? PublishedAt { get; init; }
}

public record UpdatePostRequest
{
    [Required]
    [StringLength(300, MinimumLength = 5)]
    public required string Title { get; init; }

    [Required]
    [MinLength(50)]
    public required string Content { get; init; }

    [MaxLength(500)]
    public string? Excerpt { get; init; }

    [MaxLength(500)]
    [Url]
    public string? FeaturedImageUrl { get; init; }

    public Guid? CategoryId { get; init; }

    public List<Guid> TagIds { get; init; } = [];

    // SEO
    [MaxLength(70)]
    public string? MetaTitle { get; init; }

    [MaxLength(160)]
    public string? MetaDescription { get; init; }

    public bool IsPublished { get; init; }
    public DateTime? PublishedAt { get; init; }
}

public record PostResponseDto(
    Guid Id,
    string Title,
    string Content,
    string Slug,
    string? Excerpt,
    string? FeaturedImageUrl,
    string? MetaTitle,
    string? MetaDescription,
    AuthorInfoDto Author,
    CategoryResponseDto? Category,
    List<TagResponseDto> Tags,
    bool IsPublished,
    DateTime? PublishedAt,
    int ViewCount,
    int CommentCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PostSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? FeaturedImageUrl,
    AuthorInfoDto Author,
    CategoryResponseDto? Category,
    List<TagResponseDto> Tags,
    bool IsPublished,
    DateTime? PublishedAt,
    int ViewCount,
    int CommentCount,
    DateTime CreatedAt
);

public record AuthorInfoDto(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string? AvatarUrl
);

// ==================== COMMENT DTOs ====================

public record CreateCommentRequest
{
    [Required]
    public required Guid PostId { get; init; }

    public Guid? ParentId { get; init; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public required string Content { get; init; }
}

public record UpdateCommentRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public required string Content { get; init; }
}

public record CommentResponseDto(
    Guid Id,
    Guid PostId,
    AuthorInfoDto Author,
    Guid? ParentId,
    string Content,
    bool IsApproved,
    List<CommentResponseDto> Replies,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// ==================== SEARCH & FILTER DTOs ====================

public record PostSearchRequest
{
    public string? Query { get; init; }
    public Guid? CategoryId { get; init; }
    public List<Guid> TagIds { get; init; } = [];
    public Guid? AuthorId { get; init; }
    public bool? IsPublished { get; init; } = true;
    public DateTime? PublishedAfter { get; init; }
    public DateTime? PublishedBefore { get; init; }
    public string? OrderBy { get; init; } = "publishedAt"; // publishedAt, viewCount, commentCount, title
    public bool Descending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

// ==================== STATISTICS DTOs ====================

public record BlogStatisticsDto(
    int TotalPosts,
    int PublishedPosts,
    int DraftPosts,
    int TotalComments,
    int PendingComments,
    int TotalCategories,
    int TotalTags,
    List<PopularPostDto> MostViewedPosts,
    List<PopularPostDto> MostCommentedPosts,
    List<CategoryStatsDto> CategoryStats
);

public record PopularPostDto(
    Guid Id,
    string Title,
    string Slug,
    int ViewCount,
    int CommentCount,
    DateTime? PublishedAt
);

public record CategoryStatsDto(
    Guid Id,
    string Name,
    string Slug,
    int PostCount
);

// ==================== EXTENSION METHODS ====================

public static class BlogDtoExtensions
{
    public static ProfileResponseDto ToDto(this Profile profile)
    {
        return new ProfileResponseDto(
            profile.Id,
            profile.UserId,
            profile.Bio,
            profile.AvatarUrl,
            profile.WebsiteUrl,
            profile.TwitterHandle,
            profile.GitHubHandle,
            profile.LinkedInHandle,
            profile.CreatedAt,
            profile.UpdatedAt
        );
    }

    public static CategoryResponseDto ToDto(this Category category, int postCount = 0)
    {
        return new CategoryResponseDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            postCount,
            category.CreatedAt,
            category.UpdatedAt
        );
    }

    public static TagResponseDto ToDto(this Tag tag, int postCount = 0)
    {
        return new TagResponseDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            postCount,
            tag.CreatedAt
        );
    }

    public static AuthorInfoDto ToAuthorInfo(this User user, string? avatarUrl = null)
    {
        return new AuthorInfoDto(
            user.Id,
            user.UserName,
            user.FirstName,
            user.LastName,
            avatarUrl
        );
    }

    public static PostResponseDto ToDto(this Post post, Profile? authorProfile = null)
    {
        return new PostResponseDto(
            post.Id,
            post.Title,
            post.Content,
            post.Slug,
            post.Excerpt,
            post.FeaturedImageUrl,
            post.MetaTitle,
            post.MetaDescription,
            post.Author.ToAuthorInfo(authorProfile?.AvatarUrl),
            post.Category?.ToDto(0),
            post.PostTags.Select(pt => pt.Tag.ToDto(0)).ToList(),
            post.IsPublished,
            post.PublishedAt,
            post.ViewCount,
            post.CommentCount,
            post.CreatedAt,
            post.UpdatedAt
        );
    }

    public static PostSummaryDto ToSummaryDto(this Post post, Profile? authorProfile = null)
    {
        return new PostSummaryDto(
            post.Id,
            post.Title,
            post.Slug,
            post.Excerpt,
            post.FeaturedImageUrl,
            post.Author.ToAuthorInfo(authorProfile?.AvatarUrl),
            post.Category?.ToDto(0),
            post.PostTags.Select(pt => pt.Tag.ToDto(0)).ToList(),
            post.IsPublished,
            post.PublishedAt,
            post.ViewCount,
            post.CommentCount,
            post.CreatedAt
        );
    }

    public static CommentResponseDto ToDto(this Comment comment, Profile? authorProfile = null)
    {
        return new CommentResponseDto(
            comment.Id,
            comment.PostId,
            comment.Author.ToAuthorInfo(authorProfile?.AvatarUrl),
            comment.ParentId,
            comment.Content,
            comment.IsApproved,
            comment.Replies.Select(r => r.ToDto()).ToList(),
            comment.CreatedAt,
            comment.UpdatedAt
        );
    }
}
