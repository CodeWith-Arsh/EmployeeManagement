using EmployeeApi.DTOs;
using EmployeeApi.Exceptions;
using EmployeeApi.Models;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository users, ITokenService tokenService, ILogger<AuthService> logger)
    {
        _users = users;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _users.UsernameOrEmailExistsAsync(request.Username, request.Email))
            throw new ConflictException("Username or email is already taken");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),   // salted hash
            Role = Roles.User                                                  // never trust client for role
        };

        await _users.AddAsync(user);
        _logger.LogInformation("User {Username} registered", user.Username);

        return _tokenService.CreateToken(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _users.GetByUsernameAsync(request.Username);

        // Same message for "user not found" and "wrong password" → attackers can't discover valid usernames
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password");

        _logger.LogInformation("User {Username} logged in", user.Username);
        return _tokenService.CreateToken(user);
    }
}