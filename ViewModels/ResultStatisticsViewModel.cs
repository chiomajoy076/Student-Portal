namespace Student_Portal.ViewModels;

public class CourseStatistic
{
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public string Department { get; set; }
    public string Session { get; set; }
    public string Semester { get; set; }
    public int TotalStudents { get; set; }
    public decimal AverageScore { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
}

public class ResultStatisticsViewModel
{
    public string? Session { get; set; }
    public string? Semester { get; set; }
    public string? Department { get; set; }
    public List<CourseStatistic> Courses { get; set; } = new();
}
