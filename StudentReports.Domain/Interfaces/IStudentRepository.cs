using StudentReports.Domain.Entities;
namespace StudentReports.Domain.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
    Task<IEnumerable<Student>> GetAllAsync();
}