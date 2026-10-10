using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetPagedAsync(EmployeeQueryParameters query); Task<EmployeeDto> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(EmployeeRequestDto request);
    Task UpdateAsync(int id, EmployeeRequestDto request);
    Task DeleteAsync(int id);
}