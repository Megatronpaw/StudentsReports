using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using StudentReports.Application.DTOs;
using StudentReports.Application.Interfaces;
using StudentReports.Domain.Interfaces;
using StudentReports.Infrastructure.Services.Auth;
using StudentReports.WebAPI.Controllers;
using System.Security.Claims;

namespace StudentReports.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockJwtAuthService;
    private readonly Mock<CookieAuthService> _mockCookieAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockJwtAuthService = new Mock<IAuthService>();

        
        var mockStudentRepo = new Mock<IStudentRepository>();
        var mockCache = new Mock<IMemoryCache>();


        _mockCookieAuthService = new Mock<CookieAuthService>(
            mockStudentRepo.Object,
            mockCache.Object
        );

        _controller = new AuthController(_mockJwtAuthService.Object, _mockCookieAuthService.Object);

   
        var httpContext = new DefaultHttpContext();
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
        authServiceMock
            .Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task JwtLogin_ValidCredentials_ReturnsOkWithToken()
    {

        var request = new LoginRequest { StudentId = 1, Password = "password" };
        _mockJwtAuthService.Setup(s => s.ValidateCredentials(1, "password"))
            .ReturnsAsync(true);
        _mockJwtAuthService.Setup(s => s.GenerateJwtToken(1))
            .Returns("fake-jwt-token");

   
        var result = await _controller.JwtLogin(request);

  
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal("fake-jwt-token", response.Token);
        Assert.Equal("JWT login successful", response.Message);
    }

    [Fact]
    public async Task JwtLogin_InvalidCredentials_ReturnsUnauthorized()
    {
 
        var request = new LoginRequest { StudentId = 99, Password = "wrong" };
        _mockJwtAuthService.Setup(s => s.ValidateCredentials(99, "wrong"))
            .ReturnsAsync(false);

   
        var result = await _controller.JwtLogin(request);

 
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task CookieLogin_ValidCredentials_ReturnsOkAndSetsCookie()
    {
        
        var request = new LoginRequest { StudentId = 1, Password = "password" };
        _mockCookieAuthService.Setup(s => s.ValidateCredentials(1, "password"))
            .ReturnsAsync(true);
        _mockCookieAuthService.Setup(s => s.GenerateSessionToken())
            .Returns("session-token-123");

  
        var result = await _controller.CookieLogin(request);

 
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal("Cookie login successful", response.Message);

 
        var cookies = _controller.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("SessionToken=session-token-123", cookies);
        Assert.Contains("HttpOnly", cookies);

      
        _mockCookieAuthService.Verify(s => s.StoreSession("session-token-123", 1), Times.Once);
    }

    [Fact]
    public async Task CookieLogin_InvalidCredentials_ReturnsUnauthorized()
    {

        var request = new LoginRequest { StudentId = 99, Password = "wrong" };
        _mockCookieAuthService.Setup(s => s.ValidateCredentials(99, "wrong"))
            .ReturnsAsync(false);


        var result = await _controller.CookieLogin(request);


        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Logout_ReturnsOkAndSignOut()
    {
        var result = await _controller.Logout();


        Assert.IsType<OkObjectResult>(result);

        var cookies = _controller.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("SessionToken=;", cookies);
        Assert.Contains("expires=Thu, 01 Jan 1970", cookies);
    }
}