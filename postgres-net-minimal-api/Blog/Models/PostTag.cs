using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace postgres_net_minimal_api.Models;

/// <summary>
/// Many-to-many relationship between Posts and Tags
/// </summary>
public class PostTag
{
    [Required]
    [ForeignKey(nameof(Post))]
    public Guid PostId { get; set; }

    [Required]
    [ForeignKey(nameof(Tag))]
    public Guid TagId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Post Post { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
