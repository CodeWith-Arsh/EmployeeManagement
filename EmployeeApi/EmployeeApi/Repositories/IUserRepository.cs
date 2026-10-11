using EmployeeApi.Models;

namespace EmployeeApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UsernameOrEmailExistsAsync(string username, string email);
    Task AddAsync(User user);
}