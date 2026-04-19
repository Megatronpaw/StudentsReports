using Microsoft.EntityFrameworkCore;
using StudentReports.Application.Interfaces;
using StudentReports.Domain.Entities;
using StudentReports.Infrastructure.Data;

namespace StudentReports.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context) => _context = context;

    // 1. Сравнение студента со средним по группе
    public async Task<object> GetStudentAverageComparisonAsync(int studentId)
    {
        var student = await _context.Students.Include(s => s.TestResults).FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null) return null!;

        var groupAvg = await _context.TestResults
            .Where(tr => tr.Student.GroupId == student.GroupId)
            .AverageAsync(tr => tr.IsCorrect ? 1.0 : 0.0);

        var studentAvg = student.TestResults.Any()
            ? student.TestResults.Average(tr => tr.IsCorrect ? 1.0 : 0.0)
            : 0.0;

        return new
        {
            StudentName = student.Name,
            StudentAverage = studentAvg,
            GroupAverage = groupAvg,
            Difference = studentAvg - groupAvg
        };
    }

    // 2. Топ-10 вопросов с самым низким % правильных ответов
    public async Task<object> GetTop10DifficultQuestionsAsync()
    {
        var questionStats = await _context.TestResults
            .GroupBy(tr => tr.QuestionId)
            .Select(g => new
            {
                QuestionId = g.Key,
                QuestionText = g.First().Question!.Text,
                CorrectPercentage = g.Average(tr => tr.IsCorrect ? 100.0 : 0.0)
            })
            .OrderBy(x => x.CorrectPercentage)
            .Take(10)
            .ToListAsync();

        return questionStats;
    }

    // 3. Активность студента по часам суток
    public async Task<object> GetStudentActivityByHourAsync(int studentId)
    {
        var activity = await _context.TestResults
            .Where(tr => tr.StudentId == studentId)
            .GroupBy(tr => tr.AnsweredAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderBy(x => x.Hour)
            .ToListAsync();

        return new { StudentId = studentId, Activity = activity };
    }

    // 4. Успеваемость студента с течением времени
    public async Task<object> GetStudentPerformanceOverTimeAsync(int studentId)
    {
        var results = await _context.TestResults
            .Where(tr => tr.StudentId == studentId)
            .OrderBy(tr => tr.AnsweredAt)
            .GroupBy(tr => tr.AnsweredAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                AverageScore = g.Average(tr => tr.IsCorrect ? 100.0 : 0.0)
            })
            .ToListAsync();

        return new { StudentId = studentId, Performance = results };
    }

    // 5. Средний балл группы
    public async Task<object> GetGroupAverageScoreAsync(int groupId)
    {
        var avg = await _context.TestResults
            .Where(tr => tr.Student.GroupId == groupId)
            .AverageAsync(tr => tr.IsCorrect ? 100.0 : 0.0);

        return new { GroupId = groupId, AverageScore = avg };
    }

    // 6. Распределение правильных/неправильных ответов по темам (простой вариант)
    public async Task<object> GetCorrectIncorrectByTopicAsync()
    {
        var stats = await _context.TestResults
            .GroupBy(tr => tr.Question!.Text.Substring(0, 7)) // условно по первым 7 символам
            .Select(g => new
            {
                Topic = g.Key,
                Correct = g.Count(tr => tr.IsCorrect),
                Incorrect = g.Count(tr => !tr.IsCorrect)
            })
            .ToListAsync();

        return stats;
    }

    // 7. Количество попыток на каждый вопрос (среднее по всем студентам)
    public async Task<object> GetAverageAttemptsPerQuestionAsync()
    {
        var allResults = await _context.TestResults
            .Include(tr => tr.Question)
            .Select(tr => new { tr.QuestionId, tr.StudentId, QuestionText = tr.Question!.Text })
            .ToListAsync();

        var attemptsPerQuestion = allResults
            .GroupBy(x => x.QuestionId)
            .Select(g => new
            {
                QuestionId = g.Key,
                QuestionText = g.First().QuestionText,
                AverageAttempts = g.GroupBy(x => x.StudentId)
                                     .Average(sg => sg.Count())
            })
            .ToList();

        return attemptsPerQuestion;
    }

    // 8. Cамый активный час среди всех студентов
    public async Task<object> GetGlobalPeakActivityHourAsync()
    {
        var peak = await _context.TestResults
            .GroupBy(tr => tr.AnsweredAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();

        return peak!;
    }
}