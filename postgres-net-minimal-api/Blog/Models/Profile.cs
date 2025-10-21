using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace postgres_net_minimal_api.Models;

/// <summary>
/// Extended user profile information
/// </summary>
public class Profile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    [MaxLength(100)]
    public string? TwitterHandle { get; set; }

    [MaxLength(100)]
    public string? GitHubHandle { get; set; }

    [MaxLength(100)]
    public string? LinkedInHandle { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public virtual User User { get; set; } = null!;
}
