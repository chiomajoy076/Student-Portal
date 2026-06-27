using Student_Portal.Models;

namespace Student_Portal.ViewModels;

public class CourseResultRow
{
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public int CreditUnit { get; set; }
    public int Score { get; set; }
    public Grade Grade { get; set; }
}

public class ResultCheckViewModel
{
    public string FullName { get; set; }
    public string MatricNumber { get; set; }
    public string Department { get; set; }
    public string Level { get; set; }
    public string Session { get; set; }
    public Semester Semester { get; set; }
    public List<CourseResultRow> Courses { get; set; } = new();
    public decimal GPA { get; set; }
    public decimal CGPA { get; set; }
}

public record AcademicPeriod
{
    public string Session { get; set; }
    public Semester Semester { get; set; }
    public string Label => $"{Session} - {Semester} Semester";
}
