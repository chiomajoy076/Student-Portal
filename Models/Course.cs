namespace Student_Portal.Models;

public class Course
{
    public int Id { get; set; }
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public int CreditUnit { get; set; }
    public Semester Semester { get; set; }
    public string Session { get; set; }
    public string Department { get; set; }
}

public enum Semester
{
    First = 1,
    Second
}
