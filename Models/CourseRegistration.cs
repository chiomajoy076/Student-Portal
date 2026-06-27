namespace Student_Portal.Models;

public class CourseRegistration
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public DateTime RegisteredAt { get; set; }
}
