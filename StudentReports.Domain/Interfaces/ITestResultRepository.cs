using StudentReports.Domain.Entities;
namespace StudentReports.Domain.Interfaces;

public interface ITestResultRepository
{
    Task<IEnumerable<TestResult>> GetAllAsync();
}