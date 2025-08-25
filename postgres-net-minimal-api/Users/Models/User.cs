using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace postgres_net_minimal_api.Models;


[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(20)]
    public string UserName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [MaxLength(100)]
    public string? MiddleName { get; set; } 

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } 

    [MaxLength(100)]
    public string? MotherMaidenName { get; set; } 

    [Required]
    public DateOnly DateOfBirth { get; set; } 

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string HashedPassword { get; set; } 

    [ForeignKey(nameof(Role))]
    public Guid RoleId { get; set; } 

    public virtual UserRole Role { get; set; } 
}