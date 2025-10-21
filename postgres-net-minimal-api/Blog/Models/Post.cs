using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace postgres_net_minimal_api.Models;

/// <summary>
/// Blog post
/// </summary>
[Index(nameof(Slug), IsUnique = true)]
[Index(nameof(PublishedAt))]
[Index(nameof(IsPublished))]
public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(350)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Excerpt { get; set; }

    [MaxLength(500)]
    public string? FeaturedImageUrl { get; set; }

    // SEO Metadata
    [MaxLength(70)]
    public string? MetaTitle { get; set; }

    [MaxLength(160)]
    public string? MetaDescription { get; set; }

    // Author
    [Required]
    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }

    // Category
    [ForeignKey(nameof(Category))]
    public Guid? CategoryId { get; set; }

    // Publishing
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }

    // Statistics
    public int ViewCount { get; set; }
    public int CommentCount { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual User Author { get; set; } = null!;
    public virtual Category? Category { get; set; }
    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
