using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace postgres_net_minimal_api.Models;

/// <summary>
/// Blog post tag
/// </summary>
[Index(nameof(Slug), IsUnique = true)]
public class Tag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(70)]
    public string Slug { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    // Navigation property
    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
