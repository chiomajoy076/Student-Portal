namespace Student_Portal.Models;

public class RegistrationSubmission
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Session { get; set; }
    public Semester Semester { get; set; }
    public DateTime SubmittedAt { get; set; }
}
