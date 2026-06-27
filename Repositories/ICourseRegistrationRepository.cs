using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface ICourseRegistrationRepository
{
    Task<bool> ExistsAsync(string userId, int courseId);
    Task<List<CourseRegistration>> GetByUserIdAsync(string userId);
    Task<CourseRegistration?> GetAsync(string userId, int courseId);
    Task<List<CourseRegistration>> GetByCourseIdsAsync(IEnumerable<int> courseIds);
    Task AddAsync(CourseRegistration registration);
    void Remove(CourseRegistration registration);
    Task SaveChangesAsync();
}
