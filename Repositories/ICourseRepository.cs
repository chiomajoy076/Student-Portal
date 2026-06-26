using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<Course?> FindAsync(string courseCode, string session, Semester semester);
    Task<List<Course>> GetAllAsync();
    Task AddAsync(Course course);
    Task SaveChangesAsync();
}
