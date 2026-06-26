using Microsoft.AspNetCore.Http;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IAuditLogRepository auditLogRepository, IHttpContextAccessor httpContextAccessor)
    {
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string? userId, string action)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = action,
            Timestamp = DateTime.UtcNow,
            IPAddress = ipAddress
        });

        await _auditLogRepository.SaveChangesAsync();
    }

    public async Task<List<AuditLogViewModel>> GetRecentAsync(int count = 100)
    {
        var logs = await _auditLogRepository.GetRecentAsync(count);

        return logs.Select(l => new AuditLogViewModel
        {
            Action = l.Action,
            UserEmail = l.User?.Email,
            Timestamp = l.Timestamp,
            IPAddress = l.IPAddress
        }).ToList();
    }
}
