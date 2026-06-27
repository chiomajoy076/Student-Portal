using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class CourseRegistrationRepository : ICourseRegistrationRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRegistrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(string userId, int courseId) =>
        _context.CourseRegistrations.AnyAsync(cr => cr.UserId == userId && cr.CourseId == courseId);

    public Task<List<CourseRegistration>> GetByUserIdAsync(string userId) =>
        _context.CourseRegistrations.Include(cr => cr.Course).Where(cr => cr.UserId == userId).ToListAsync();

    public Task<CourseRegistration?> GetAsync(string userId, int courseId) =>
        _context.CourseRegistrations.Include(cr => cr.Course)
            .FirstOrDefaultAsync(cr => cr.UserId == userId && cr.CourseId == courseId);

    public Task<List<CourseRegistration>> GetByCourseIdsAsync(IEnumerable<int> courseIds) =>
        _context.CourseRegistrations
            .Include(cr => cr.Course)
            .Include(cr => cr.User)
            .Where(cr => courseIds.Contains(cr.CourseId))
            .ToListAsync();

    public async Task AddAsync(CourseRegistration registration)
    {
        await _context.CourseRegistrations.AddAsync(registration);
    }

    public void Remove(CourseRegistration registration)
    {
        _context.CourseRegistrations.Remove(registration);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
