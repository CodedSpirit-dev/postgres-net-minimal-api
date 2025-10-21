using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace postgres_net_minimal_api.Authorization.Models;

/// <summary>
/// Represents an action that can be performed (e.g., "View", "Create", "Edit")
/// </summary>
public class PermissionAction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ActionKey { get; set; } = string.Empty; // Maps to ActionType enum

    [MaxLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<FeatureAction> FeatureActions { get; set; } = new List<FeatureAction>();
}
