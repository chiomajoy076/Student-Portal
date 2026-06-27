using Student_Portal.Models;

namespace Student_Portal.ViewModels;

public class AvailableCourseViewModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public int CreditUnit { get; set; }
    public string Session { get; set; }
    public Semester Semester { get; set; }
    public bool IsRegistered { get; set; }
    public bool IsFinalized { get; set; }
}

public class StudentRegisteredCourseViewModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public string Session { get; set; }
    public Semester Semester { get; set; }
}
