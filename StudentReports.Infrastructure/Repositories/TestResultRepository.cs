using Microsoft.EntityFrameworkCore;
using StudentReports.Domain.Entities;
using StudentReports.Domain.Interfaces;
using StudentReports.Infrastructure.Data;

namespace StudentReports.Infrastructure.Repositories;

public class TestResultRepository : ITestResultRepository
{
    private readonly AppDbContext _context;
    public TestResultRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<TestResult>> GetAllAsync() =>
        await _context.TestResults.Include(tr => tr.Student).Include(tr => tr.Question).ToListAsync();

    public async Task<IEnumerable<TestResult>> GetByStudentIdAsync(int studentId) =>
        await _context.TestResults.Where(tr => tr.StudentId == studentId).ToListAsync();
}