using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Data;

namespace postgres_net_minimal_api.Authorization.Services;

/// <summary>
/// Checks if a user owns a specific comment
/// </summary>
public class CommentOwnershipChecker(AppDbContext context) : IResourceOwnershipChecker
{
    public async Task<bool> UserOwnsResourceAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken = default)
    {
        return await context.Comments
            .AsNoTracking()
            .AnyAsync(c => c.Id == resourceId && c.AuthorId == userId, cancellationToken);
    }
}
