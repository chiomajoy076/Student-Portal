namespace Student_Portal.ViewModels;

public class ResultImportSummaryViewModel
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public List<string> RowErrors { get; set; } = new();
}
