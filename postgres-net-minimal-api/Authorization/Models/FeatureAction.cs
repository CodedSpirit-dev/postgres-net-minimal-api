using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace postgres_net_minimal_api.Authorization.Models;

/// <summary>
/// Represents a permission (combination of Feature + Action)
/// Example: Posts.View, Posts.Create, Users.Delete
/// </summary>
public class FeatureAction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Feature))]
    public int FeatureId { get; set; }

    [Required]
    [ForeignKey(nameof(Action))]
    public int ActionId { get; set; }

    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Feature Feature { get; set; } = null!;
    public virtual PermissionAction Action { get; set; } = null!;
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
