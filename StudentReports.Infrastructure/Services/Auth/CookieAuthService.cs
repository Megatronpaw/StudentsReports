using Microsoft.Extensions.Caching.Memory;
using StudentReports.Application.Interfaces;
using StudentReports.Domain.Interfaces;

namespace StudentReports.Infrastructure.Services.Auth;

public class CookieAuthService : IAuthService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IMemoryCache _cache;

    public CookieAuthService(IStudentRepository studentRepo, IMemoryCache cache)
    {
        _studentRepo = studentRepo;
        _cache = cache;
    }

    public virtual async Task<bool> ValidateCredentials(int studentId, string password)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        return student != null && password == "password";
    }

    public string GenerateJwtToken(int studentId) => throw new NotImplementedException();

    public virtual string GenerateSessionToken()
    {
        var token = Guid.NewGuid().ToString();
        return token;
    }

    public virtual void StoreSession(string sessionToken, int studentId)
    {
        _cache.Set(sessionToken, studentId, TimeSpan.FromHours(1));
    }

    public virtual int? GetStudentIdFromSession(string sessionToken)
    {
        _cache.TryGetValue(sessionToken, out int studentId);
        return studentId == 0 ? null : studentId;
    }
}