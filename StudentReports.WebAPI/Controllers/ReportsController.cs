using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentReports.Application.Interfaces;
using System.Security.Claims;

namespace StudentReports.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    private int GetCurrentStudentId()
    {
        var claim = User.FindFirst("studentId")?.Value;
        return claim != null ? int.Parse(claim) : 0;
    }

    [HttpGet("my-comparison")]
    public async Task<IActionResult> GetMyComparison()
    {
        var studentId = GetCurrentStudentId();
        var result = await _reportService.GetStudentAverageComparisonAsync(studentId);
        return Ok(result);
    }

    [HttpGet("top10-difficult")]
    public async Task<IActionResult> GetTop10Difficult() => Ok(await _reportService.GetTop10DifficultQuestionsAsync());

    [HttpGet("my-activity-by-hour")]
    public async Task<IActionResult> GetMyActivityByHour()
    {
        var studentId = GetCurrentStudentId();
        return Ok(await _reportService.GetStudentActivityByHourAsync(studentId));
    }

    [HttpGet("my-performance-over-time")]
    public async Task<IActionResult> GetMyPerformanceOverTime()
    {
        var studentId = GetCurrentStudentId();
        return Ok(await _reportService.GetStudentPerformanceOverTimeAsync(studentId));
    }

    [HttpGet("group-average/{groupId}")]
    public async Task<IActionResult> GetGroupAverage(int groupId) => Ok(await _reportService.GetGroupAverageScoreAsync(groupId));

    [HttpGet("correct-incorrect-by-topic")]
    public async Task<IActionResult> GetCorrectIncorrectByTopic() => Ok(await _reportService.GetCorrectIncorrectByTopicAsync());

    [HttpGet("average-attempts-per-question")]
    public async Task<IActionResult> GetAverageAttemptsPerQuestion() => Ok(await _reportService.GetAverageAttemptsPerQuestionAsync());

    [HttpGet("peak-activity-hour")]
    public async Task<IActionResult> GetPeakActivityHour() => Ok(await _reportService.GetGlobalPeakActivityHourAsync());
}