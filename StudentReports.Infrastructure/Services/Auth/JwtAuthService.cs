using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentReports.Application.Interfaces;
using StudentReports.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentReports.Infrastructure.Services.Auth;

public class JwtAuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IStudentRepository _studentRepo;

    public JwtAuthService(IConfiguration configuration, IStudentRepository studentRepo)
    {
        _configuration = configuration;
        _studentRepo = studentRepo;
    }

    public async Task<bool> ValidateCredentials(int studentId, string password)
    {
        // Упрощённая проверка – пароль всегда "password"
        var student = await _studentRepo.GetByIdAsync(studentId);
        return student != null && password == "password";
    }

    public string GenerateJwtToken(int studentId)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("studentId", studentId.ToString()) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateSessionToken() => Guid.NewGuid().ToString();
}