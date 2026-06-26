namespace Student_Portal.ViewModels;

public class AuditLogViewModel
{
    public string Action { get; set; }
    public string? UserEmail { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IPAddress { get; set; }
}
