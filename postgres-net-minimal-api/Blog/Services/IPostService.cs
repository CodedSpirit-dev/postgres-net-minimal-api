using postgres_net_minimal_api.Blog.DTOs;
using postgres_net_minimal_api.Helpers;

namespace postgres_net_minimal_api.Blog.Services;

public interface IPostService
{
    Task<PagedResult<PostSummaryDto>> GetAllPostsAsync(
        int page,
        int pageSize,
        bool? isPublished = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PostSummaryDto>> SearchPostsAsync(
        PostSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<PostResponseDto?> GetPostByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PostResponseDto?> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<PostResponseDto> CreatePostAsync(
        CreatePostRequest request,
        Guid authorId,
        CancellationToken cancellationToken = default);

    Task<PostResponseDto?> UpdatePostAsync(
        Guid id,
        UpdatePostRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PostResponseDto?> PublishPostAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PostResponseDto?> UnpublishPostAsync(Guid id, CancellationToken cancellationToken = default);

    Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<PopularPostDto>> GetMostViewedPostsAsync(
        int count = 10,
        CancellationToken cancellationToken = default);

    Task<List<PopularPostDto>> GetMostCommentedPostsAsync(
        int count = 10,
        CancellationToken cancellationToken = default);
}
