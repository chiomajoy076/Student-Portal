using Student_Portal.Models;

namespace Student_Portal.ViewModels;

public class RosterStudentRow
{
    public string FullName { get; set; }
    public string MatricNumber { get; set; }
    public string Email { get; set; }
}

public class CourseRosterViewModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public string Department { get; set; }
    public string Session { get; set; }
    public Semester Semester { get; set; }
    public List<RosterStudentRow> Students { get; set; } = new();
}
