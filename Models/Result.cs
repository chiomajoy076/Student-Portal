namespace Student_Portal.Models;

public class Result
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public int Score { get; set; }
    public Grade Grade { get; set; }
    public DateTime UploadedAt { get; set; }
}

public enum Grade
{
    F = 0,
    E = 1,
    D = 2,
    C = 3,
    B = 4,
    A = 5
}
