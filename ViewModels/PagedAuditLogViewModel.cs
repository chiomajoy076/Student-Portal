namespace Student_Portal.ViewModels;

public class PagedAuditLogViewModel
{
    public List<AuditLogViewModel> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public string? EmailFilter { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
