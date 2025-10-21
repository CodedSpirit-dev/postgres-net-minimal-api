using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace postgres_net_minimal_api.Authorization.Models;

/// <summary>
/// Represents a high-level application module (e.g., "User Management", "Blog System")
/// </summary>
public class ApplicationModule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<ModuleFeature> ModuleFeatures { get; set; } = new List<ModuleFeature>();
}
