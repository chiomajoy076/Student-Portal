namespace Student_Portal.ViewModels;

public class RegistrationRow
{
    public string MatricNumber { get; set; }
    public string FullName { get; set; }
    public string Department { get; set; }
    public string Level { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public bool IsSubmitted { get; set; }
}

public class RegistrationReportViewModel
{
    public string? DepartmentFilter { get; set; }
    public List<RegistrationRow> Rows { get; set; } = new();
    public Dictionary<string, int> CountsByDepartment { get; set; } = new();
    public int TotalCount { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public List<RegistrationRow> PagedRows => Rows.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
}
