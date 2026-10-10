using EmployeeApi.DTOs;
using EmployeeApi.Models;

namespace EmployeeApi.Mappings;

public static class EmployeeMappings
{
    // Entity → Response DTO
    public static EmployeeDto ToDto(this Employee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        Department = e.Department,
        Salary = e.Salary,
        DateOfJoining = e.DateOfJoining
    };

    // Request DTO → new Entity (for Create)
    public static Employee ToEntity(this EmployeeRequestDto r) => new()
    {
        FirstName = r.FirstName,
        LastName = r.LastName,
        Email = r.Email,
        Department = r.Department,
        Salary = r.Salary,
        DateOfJoining = r.DateOfJoining
    };

    // Copy Request DTO values onto an existing Entity (for Update)
    public static void UpdateFrom(this Employee e, EmployeeRequestDto r)
    {
        e.FirstName = r.FirstName;
        e.LastName = r.LastName;
        e.Email = r.Email;
        e.Department = r.Department;
        e.Salary = r.Salary;
        e.DateOfJoining = r.DateOfJoining;
    }
}