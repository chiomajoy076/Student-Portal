using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface IGpaRecordRepository
{
    Task<List<GpaRecord>> GetByUserIdAsync(string userId);
    Task<GpaRecord?> GetByUserSessionSemesterAsync(string userId, string session, Semester semester);
    Task AddAsync(GpaRecord record);
    Task SaveChangesAsync();
}
