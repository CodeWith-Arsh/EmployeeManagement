using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    // NEVER store plain passwords – only the BCrypt hash
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = Roles.User;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Constants avoid typos like "admin" vs "Admin"
public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}