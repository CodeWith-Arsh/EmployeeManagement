using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.DTOs;

public class EmployeeRequestDto
{
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Department { get; set; } = string.Empty;

    [Range(0, 10000000, ErrorMessage = "Salary must be between 0 and 1 crore")]
    public decimal Salary { get; set; }

    [Required]
    public DateTime DateOfJoining { get; set; }
}