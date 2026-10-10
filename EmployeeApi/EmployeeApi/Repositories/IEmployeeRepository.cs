using EmployeeApi.DTOs;
using EmployeeApi.Models;


namespace EmployeeApi.Repositories;

public interface IEmployeeRepository
{
    Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeQueryParameters query); Task<Employee?> GetByIdAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
}