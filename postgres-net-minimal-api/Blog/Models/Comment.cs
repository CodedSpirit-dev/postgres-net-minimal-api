using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace postgres_net_minimal_api.Models;

/// <summary>
/// Comment on a blog post
/// </summary>
[Index(nameof(PostId))]
[Index(nameof(IsApproved))]
public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Post))]
    public Guid PostId { get; set; }

    [Required]
    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }

    // For nested comments (replies)
    [ForeignKey(nameof(ParentComment))]
    public Guid? ParentId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public bool IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Post Post { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
