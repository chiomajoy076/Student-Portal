using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class ResultRepository : IResultRepository
{
    private readonly ApplicationDbContext _context;

    public ResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(string userId, int courseId) =>
        _context.Results.AnyAsync(r => r.UserId == userId && r.CourseId == courseId);

    public Task<List<Result>> GetByUserIdAsync(string userId) =>
        _context.Results.Include(r => r.Course).Where(r => r.UserId == userId).ToListAsync();

    public async Task AddAsync(Result result)
    {
        await _context.Results.AddAsync(result);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
