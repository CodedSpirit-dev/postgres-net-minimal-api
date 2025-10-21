using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace postgres_net_minimal_api.Authorization.Models;

/// <summary>
/// Represents a specific feature or resource (e.g., "Posts", "Users")
/// </summary>
public class Feature
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ResourceKey { get; set; } = string.Empty; // Maps to ResourceType enum

    [MaxLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<ModuleFeature> ModuleFeatures { get; set; } = new List<ModuleFeature>();
    public virtual ICollection<FeatureAction> FeatureActions { get; set; } = new List<FeatureAction>();
}
