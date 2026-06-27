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

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling(Courses.Count / (double)PageSize);
    public List<CourseStatistic> PagedCourses => Courses.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
}
