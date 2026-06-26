namespace Student_Portal.Models;

public class GpaRecord
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Session { get; set; }
    public Semester Semester { get; set; }
    public decimal GPA { get; set; }
    public decimal CGPA { get; set; }
    public DateTime ComputedAt { get; set; }
}
