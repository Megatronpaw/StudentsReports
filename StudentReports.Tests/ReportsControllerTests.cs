using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentReports.Application.Interfaces;
using StudentReports.WebAPI.Controllers;
using System.Security.Claims;

namespace StudentReports.Tests;

public class ReportsControllerTests
{
    private readonly Mock<IReportService> _mockReportService;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _mockReportService = new Mock<IReportService>();
        _controller = new ReportsController(_mockReportService.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("studentId", "1")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetMyComparison_ReturnsOkResult()
    {
        // Arrange
        _mockReportService.Setup(s => s.GetStudentAverageComparisonAsync(1))
            .ReturnsAsync(new { StudentName = "Test" });

        // Act
        var result = await _controller.GetMyComparison();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetTop10Difficult_ReturnsOkResult()
    {
        _mockReportService.Setup(s => s.GetTop10DifficultQuestionsAsync())
            .ReturnsAsync(new List<object>());

        var result = await _controller.GetTop10Difficult();
        Assert.IsType<OkObjectResult>(result);
    }

}