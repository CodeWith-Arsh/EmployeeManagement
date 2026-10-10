using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.DTOs;

public class EmployeeQueryParameters
{
    // Search across first name, last name and email
    public string? Search { get; set; }

    // Exact filter
    public string? Department { get; set; }

    // firstName | lastName | email | department | salary | dateOfJoining (default: id)
    public string? SortBy { get; set; }

    public bool Descending { get; set; } = false;

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be 1 or more")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50")]
    public int PageSize { get; set; } = 10;
}