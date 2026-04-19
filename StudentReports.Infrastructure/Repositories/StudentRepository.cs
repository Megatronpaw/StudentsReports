using Microsoft.EntityFrameworkCore;
using StudentReports.Domain.Entities;
using StudentReports.Domain.Interfaces;
using StudentReports.Infrastructure.Data;

namespace StudentReports.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;
    public StudentRepository(AppDbContext context) => _context = context;

    public async Task<Student?> GetByIdAsync(int id) =>
        await _context.Students.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Student>> GetAllAsync() =>
        await _context.Students.Include(s => s.Group).ToListAsync();
}