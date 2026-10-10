using EmployeeApi.DTOs;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    // GET: api/employees?search=arsh&department=IT&sortBy=salary&descending=true&pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetAll([FromQuery] EmployeeQueryParameters query)
        => Ok(await _service.GetPagedAsync(query));

    // GET: api/employees/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeRequestDto request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/employees/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeRequestDto request)
    {
        await _service.UpdateAsync(id, request);
        return NoContent();
    }

    // DELETE: api/employees/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}