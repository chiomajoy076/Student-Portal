using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class GpaRecordRepository : IGpaRecordRepository
{
    private readonly ApplicationDbContext _context;

    public GpaRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<GpaRecord>> GetByUserIdAsync(string userId) =>
        _context.GpaRecords.Where(g => g.UserId == userId).ToListAsync();

    public Task<GpaRecord?> GetByUserSessionSemesterAsync(string userId, string session, Semester semester) =>
        _context.GpaRecords.FirstOrDefaultAsync(g =>
            g.UserId == userId && g.Session == session && g.Semester == semester);

    public async Task AddAsync(GpaRecord record)
    {
        await _context.GpaRecords.AddAsync(record);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
