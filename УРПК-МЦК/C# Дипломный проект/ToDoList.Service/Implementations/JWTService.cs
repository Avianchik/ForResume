using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Domain.Entity;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public class JWTService
{
    private readonly JWTOptions _options;
    private ILogger<JWTService> _logger;
    
    public JWTService(IOptions<JWTOptions> options, ILogger<JWTService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }
    
    public string GenerateToken(UserEntity user)
    {
        Claim[] claims = [new("userId", user.Id.ToString())];
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires:DateTime.UtcNow.AddHours(_options.ExpiresHours));

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }
}