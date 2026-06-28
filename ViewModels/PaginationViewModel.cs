namespace Student_Portal.ViewModels;

public class PaginationViewModel
{
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public string Action { get; set; } = "Index";
    public IDictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();
}
