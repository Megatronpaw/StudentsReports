using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudentReports.Application.DTOs;
using StudentReports.Application.Interfaces;
using StudentReports.Infrastructure.Services.Auth;
using System.Security.Claims;

namespace StudentReports.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _jwtAuthService;
    private readonly CookieAuthService _cookieAuthService;

    public AuthController(IAuthService jwtAuthService, CookieAuthService cookieAuthService)
    {
        _jwtAuthService = jwtAuthService;
        _cookieAuthService = cookieAuthService;
    }

    [HttpPost("jwt-login")]
    public async Task<IActionResult> JwtLogin([FromBody] LoginRequest request)
    {
        if (await _jwtAuthService.ValidateCredentials(request.StudentId, request.Password))
        {
            var token = _jwtAuthService.GenerateJwtToken(request.StudentId);
            return Ok(new LoginResponse { Token = token, Message = "JWT login successful" });
        }
        return Unauthorized("Invalid credentials");
    }

    [HttpPost("cookie-login")]
    public async Task<IActionResult> CookieLogin([FromBody] LoginRequest request)
    {
        if (await _cookieAuthService.ValidateCredentials(request.StudentId, request.Password))
        {
            var sessionToken = _cookieAuthService.GenerateSessionToken();
            _cookieAuthService.StoreSession(sessionToken, request.StudentId);

            var claims = new List<Claim> { new Claim("studentId", request.StudentId.ToString()) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) });

            Response.Cookies.Append("SessionToken", sessionToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(new LoginResponse { Message = "Cookie login successful" });
        }
        return Unauthorized("Invalid credentials");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("SessionToken");
        return Ok("Logged out");
    }
}