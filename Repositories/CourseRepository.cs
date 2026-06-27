using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Course?> GetByIdAsync(int id) => _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

    public Task<Course?> FindAsync(string courseCode, string session, Semester semester) =>
        _context.Courses.FirstOrDefaultAsync(c =>
            c.CourseCode == courseCode && c.Session == session && c.Semester == semester);

    public Task<List<Course>> GetAllAsync() => _context.Courses.OrderBy(c => c.CourseCode).ToListAsync();

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
