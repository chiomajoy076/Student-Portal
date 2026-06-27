using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public Task<List<AuditLog>> GetRecentAsync(int count) =>
        _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();

    public async Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? emailFilter = null)
    {
        var query = _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(emailFilter))
        {
            query = query.Where(a => a.User != null && a.User.Email != null && a.User.Email.Contains(emailFilter));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
