using System.Text.RegularExpressions;

namespace StudentReports.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public Group? Group { get; set; }
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}