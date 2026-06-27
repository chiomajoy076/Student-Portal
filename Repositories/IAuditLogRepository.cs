using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task SaveChangesAsync();
    Task<List<AuditLog>> GetRecentAsync(int count);
}
