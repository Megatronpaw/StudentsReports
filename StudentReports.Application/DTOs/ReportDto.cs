namespace StudentReports.Application.DTOs;

public class ReportDto<T>
{
    public string ReportName { get; set; } = string.Empty;
    public T Data { get; set; } = default!;
}