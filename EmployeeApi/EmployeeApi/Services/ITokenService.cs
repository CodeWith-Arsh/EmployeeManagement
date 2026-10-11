using EmployeeApi.DTOs;
using EmployeeApi.Models;

namespace EmployeeApi.Services;

public interface ITokenService
{
    AuthResponseDto CreateToken(User user);
}