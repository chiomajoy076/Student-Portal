using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface IResultRepository
{
    Task<bool> ExistsAsync(string userId, int courseId);
    Task<List<Result>> GetByUserIdAsync(string userId);
    Task AddAsync(Result result);
    Task SaveChangesAsync();
}
