namespace StudentReports.Application.Interfaces;

public interface IAuthService
{
    Task<bool> ValidateCredentials(int studentId, string password);
    string GenerateJwtToken(int studentId);
    string GenerateSessionToken();
}