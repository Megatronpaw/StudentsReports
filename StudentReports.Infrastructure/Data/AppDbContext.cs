using Microsoft.EntityFrameworkCore;
using StudentReports.Domain.Entities;

namespace StudentReports.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<TestResult> TestResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId);

        modelBuilder.Entity<TestResult>()
            .HasOne(tr => tr.Student)
            .WithMany(s => s.TestResults)
            .HasForeignKey(tr => tr.StudentId);

        modelBuilder.Entity<TestResult>()
            .HasOne(tr => tr.Question)
            .WithMany(q => q.TestResults)
            .HasForeignKey(tr => tr.QuestionId);

        
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Group>().HasData(
            new Group { Id = 1, Name = "Группа А" },
            new Group { Id = 2, Name = "Группа Б" }
        );

       
        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, Name = "Иван Иванов", GroupId = 1 },
            new Student { Id = 2, Name = "Петр Петров", GroupId = 1 },
            new Student { Id = 3, Name = "Мария Сидорова", GroupId = 2 }
        );

        
        var questions = new List<Question>();
        for (int i = 1; i <= 20; i++)
            questions.Add(new Question { Id = i, Text = $"Вопрос {i}" });
        modelBuilder.Entity<Question>().HasData(questions);

        var random = new Random();
        var testResults = new List<TestResult>();
        int id = 1;
        for (int student = 1; student <= 3; student++)
        {
            for (int q = 1; q <= 20; q++)
            {
                for (int attempt = 1; attempt <= random.Next(1, 5); attempt++)
                {
                    testResults.Add(new TestResult
                    {
                        Id = id++,
                        StudentId = student,
                        QuestionId = q,
                        IsCorrect = random.Next(0, 100) > 40, 
                        AnsweredAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)).AddHours(random.Next(0, 23))
                    });
                }
            }
        }
        modelBuilder.Entity<TestResult>().HasData(testResults);
    }
}