namespace StudentReports.Application.DTOs;

public class LoginRequest
{
    public int StudentId { get; set; }
    public string Password { get; set; } = string.Empty;
}