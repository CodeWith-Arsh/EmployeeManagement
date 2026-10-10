using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(EmployeeRequestDto request);
    Task UpdateAsync(int id, EmployeeRequestDto request);
    Task DeleteAsync(int id);
}