using System.Threading.Tasks;

namespace StudentReports.Application.Interfaces;

public interface IReportService
{
    Task<object> GetStudentAverageComparisonAsync(int studentId);
    Task<object> GetTop10DifficultQuestionsAsync();
    Task<object> GetStudentActivityByHourAsync(int studentId);
    Task<object> GetStudentPerformanceOverTimeAsync(int studentId);
    Task<object> GetGroupAverageScoreAsync(int groupId);
    Task<object> GetCorrectIncorrectByTopicAsync();
    Task<object> GetAverageAttemptsPerQuestionAsync();
    Task<object> GetGlobalPeakActivityHourAsync();
}