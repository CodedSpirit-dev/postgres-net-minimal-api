using postgres_net_minimal_api.Models;
using postgres_net_minimal_api.Common.Specifications;

namespace postgres_net_minimal_api.Blog.Specifications;

/// <summary>
/// Specification for published posts with full details (OCP)
/// Easy to extend without modifying existing code
/// </summary>
public class PublishedPostsSpecification : BaseSpecification<Post>
{
    public PublishedPostsSpecification() : base(p => p.IsPublished)
    {
        AddInclude(p => p.Author);
        AddInclude(p => p.Category);
        AddInclude("PostTags.Tag"); // String include for nested navigation
        ApplyOrderByDescending(p => p.PublishedAt);
    }
}

/// <summary>
/// Specification for paginated posts
/// </summary>
public class PaginatedPostsSpecification : BaseSpecification<Post>
{
    public PaginatedPostsSpecification(
        int page,
        int pageSize,
        bool? isPublished = null) : base(p => isPublished == null || p.IsPublished == isPublished)
    {
        AddInclude(p => p.Author);
        AddInclude(p => p.Category);
        AddInclude("PostTags.Tag");
        ApplyOrderByDescending(p => p.PublishedAt);
        ApplyPaging((page - 1) * pageSize, pageSize);
    }
}

/// <summary>
/// Specification for post by slug
/// </summary>
public class PostBySlugSpecification : BaseSpecification<Post>
{
    public PostBySlugSpecification(string slug) : base(p => p.Slug == slug)
    {
        AddInclude(p => p.Author);
        AddInclude(p => p.Category);
        AddInclude("PostTags.Tag");
    }
}

/// <summary>
/// Specification for most viewed posts
/// </summary>
public class MostViewedPostsSpecification : BaseSpecification<Post>
{
    public MostViewedPostsSpecification(int count) : base(p => p.IsPublished)
    {
        ApplyOrderByDescending(p => p.ViewCount);
        ApplyPaging(0, count);
    }
}

/// <summary>
/// Specification for most commented posts
/// </summary>
public class MostCommentedPostsSpecification : BaseSpecification<Post>
{
    public MostCommentedPostsSpecification(int count) : base(p => p.IsPublished)
    {
        ApplyOrderByDescending(p => p.CommentCount);
        ApplyPaging(0, count);
    }
}

/// <summary>
/// Specification for posts by category
/// </summary>
public class PostsByCategorySpecification : BaseSpecification<Post>
{
    public PostsByCategorySpecification(Guid categoryId, bool publishedOnly = true)
        : base(p => p.CategoryId == categoryId && (!publishedOnly || p.IsPublished))
    {
        AddInclude(p => p.Author);
        AddInclude(p => p.Category);
        AddInclude("PostTags.Tag");
        ApplyOrderByDescending(p => p.PublishedAt);
    }
}

/// <summary>
/// Specification for posts by author
/// </summary>
public class PostsByAuthorSpecification : BaseSpecification<Post>
{
    public PostsByAuthorSpecification(Guid authorId, bool publishedOnly = true)
        : base(p => p.AuthorId == authorId && (!publishedOnly || p.IsPublished))
    {
        AddInclude(p => p.Author);
        AddInclude(p => p.Category);
        AddInclude("PostTags.Tag");
        ApplyOrderByDescending(p => p.PublishedAt);
    }
}
