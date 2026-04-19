namespace StudentReports.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}