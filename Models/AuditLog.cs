namespace Student_Portal.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string Action { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IPAddress { get; set; }
}
