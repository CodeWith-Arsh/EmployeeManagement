using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    // Controller depends on the INTERFACE, not on DbContext
    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var employees = await _repository.GetAllAsync();
        return Ok(employees.Select(ToDto));
    }

    // GET: api/employees/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee is null)
            return NotFound(new { message = $"Employee with id {id} not found" });

        return Ok(ToDto(employee));
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeRequestDto request)
    {
        if (await _repository.EmailExistsAsync(request.Email))
            return Conflict(new { message = "An employee with this email already exists" });

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Department = request.Department,
            Salary = request.Salary,
            DateOfJoining = request.DateOfJoining
        };

        var created = await _repository.AddAsync(employee);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    // PUT: api/employees/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeRequestDto request)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee is null)
            return NotFound(new { message = $"Employee with id {id} not found" });

        if (await _repository.EmailExistsAsync(request.Email, id))
            return Conflict(new { message = "Another employee already uses this email" });

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.Department = request.Department;
        employee.Salary = request.Salary;
        employee.DateOfJoining = request.DateOfJoining;

        await _repository.UpdateAsync(employee);
        return NoContent();
    }

    // DELETE: api/employees/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee is null)
            return NotFound(new { message = $"Employee with id {id} not found" });

        await _repository.DeleteAsync(employee);
        return NoContent();
    }

    private static EmployeeDto ToDto(Employee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        Department = e.Department,
        Salary = e.Salary,
        DateOfJoining = e.DateOfJoining
    };
}