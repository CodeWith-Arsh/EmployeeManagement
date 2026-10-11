using System.Security.Claims;
using System.Text;
using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeApi.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwt;

    public TokenService(IOptions<JwtOptions> options)
    {
        _jwt = options.Value;
    }

    public AuthResponseDto CreateToken(User user)
    {
        // 1. Claims = facts about the user stored inside the token
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),          // subject = user id
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.Role, user.Role),                               // used by [Authorize(Roles = ...)]
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())    // unique token id
        };

        // 2. Sign with the secret key (HMAC-SHA256)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

        // 3. Build the token
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            SigningCredentials = credentials
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            Username = user.Username,
            Role = user.Role
        };
    }
}