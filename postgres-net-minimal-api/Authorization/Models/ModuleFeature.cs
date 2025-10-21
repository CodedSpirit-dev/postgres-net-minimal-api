using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace postgres_net_minimal_api.Authorization.Models;

/// <summary>
/// Many-to-many relationship between Modules and Features
/// </summary>
public class ModuleFeature
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Module))]
    public int ModuleId { get; set; }

    [Required]
    [ForeignKey(nameof(Feature))]
    public int FeatureId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual ApplicationModule Module { get; set; } = null!;
    public virtual Feature Feature { get; set; } = null!;
}
