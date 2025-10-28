using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Roles.Models;

namespace postgres_net_minimal_api.Users.Models;


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

    [MaxLength(500)]
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    [NotMapped]
    public string FullName => string.IsNullOrEmpty(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    [NotMapped]
    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}