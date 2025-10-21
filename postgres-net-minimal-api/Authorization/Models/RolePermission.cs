using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Authorization.Models;

/// <summary>
/// Assigns permissions (FeatureActions) to roles
/// </summary>
public class RolePermission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Role))]
    public Guid RoleId { get; set; }

    [Required]
    [ForeignKey(nameof(FeatureAction))]
    public int FeatureActionId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual UserRole Role { get; set; } = null!;
    public virtual FeatureAction FeatureAction { get; set; } = null!;
}
