namespace StudentReports.Domain.Entities;

public class TestResult
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; }
}