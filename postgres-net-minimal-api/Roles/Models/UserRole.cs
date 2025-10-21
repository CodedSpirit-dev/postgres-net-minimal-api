using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using postgres_net_minimal_api.Authorization.Models;

namespace postgres_net_minimal_api.Models;

public class UserRole
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    // Navigation property for granular permissions
    public virtual ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}