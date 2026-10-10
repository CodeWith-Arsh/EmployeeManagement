using EmployeeApi.DTOs;
using EmployeeApi.Exceptions;
using EmployeeApi.Mappings;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(EmployeeQueryParameters query)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(query);

        return new PagedResult<EmployeeDto>
        {
            Items = items.Select(e => e.ToDto()).ToList(),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<EmployeeDto> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee with id {id} not found");

        return employee.ToDto();
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeRequestDto request)
    {
        if (await _repository.EmailExistsAsync(request.Email))
            throw new ConflictException("An employee with this email already exists");

        var created = await _repository.AddAsync(request.ToEntity());

        _logger.LogInformation("Employee {EmployeeId} created", created.Id);
        return created.ToDto();
    }

    public async Task UpdateAsync(int id, EmployeeRequestDto request)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee with id {id} not found");

        if (await _repository.EmailExistsAsync(request.Email, id))
            throw new ConflictException("Another employee already uses this email");

        employee.UpdateFrom(request);
        await _repository.UpdateAsync(employee);

        _logger.LogInformation("Employee {EmployeeId} updated", id);
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee with id {id} not found");

        await _repository.DeleteAsync(employee);

        _logger.LogInformation("Employee {EmployeeId} deleted", id);
    }
}