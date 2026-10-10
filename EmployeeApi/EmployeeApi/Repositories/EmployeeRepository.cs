using EmployeeApi.Data;
using EmployeeApi.DTOs;
using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeQueryParameters q)
    {
        // IQueryable = query is BUILT here but NOT executed yet
        IQueryable<Employee> query = _context.Employees.AsNoTracking();

        // ---------- 1. SEARCH ----------
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var term = q.Search.Trim();
            query = query.Where(e =>
                e.FirstName.Contains(term) ||
                e.LastName.Contains(term) ||
                e.Email.Contains(term));
        }

        // ---------- 2. FILTER ----------
        if (!string.IsNullOrWhiteSpace(q.Department))
            query = query.Where(e => e.Department == q.Department);

        // ---------- 3. COUNT (before paging) ----------
        var totalCount = await query.CountAsync();

        // ---------- 4. SORT ----------
        query = q.SortBy?.ToLowerInvariant() switch
        {
            "firstname" => q.Descending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
            "lastname" => q.Descending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
            "email" => q.Descending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
            "department" => q.Descending ? query.OrderByDescending(e => e.Department) : query.OrderBy(e => e.Department),
            "salary" => q.Descending ? query.OrderByDescending(e => e.Salary) : query.OrderBy(e => e.Salary),
            "dateofjoining" => q.Descending ? query.OrderByDescending(e => e.DateOfJoining) : query.OrderBy(e => e.DateOfJoining),
            _ => q.Descending ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id)
        };

        // ---------- 5. PAGE ----------
        var items = await query
            .Skip((q.PageNumber - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync();          // ← SQL runs HERE

        return (items, totalCount);
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _context.Employees
            .AnyAsync(e => e.Email == email && (excludeId == null || e.Id != excludeId));
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;                 // Id is now filled by SQL Server
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}