using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;

namespace postgres_net_minimal_api.Authorization.Services;

/// <summary>
/// Checks if a user owns a specific post
/// </summary>
public class PostOwnershipChecker(AppDbContext context) : IResourceOwnershipChecker
{
    public async Task<bool> UserOwnsResourceAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken = default)
    {
        return await context.Posts
            .AsNoTracking()
            .AnyAsync(p => p.Id == resourceId && p.AuthorId == userId, cancellationToken);
    }
}
