using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IAuditService
{
    Task LogAsync(string? userId, string action);
    Task<List<AuditLogViewModel>> GetRecentAsync(int count = 100);
}
