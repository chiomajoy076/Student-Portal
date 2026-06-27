namespace Student_Portal.Models;

public class LecturerDepartment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Department { get; set; }
}
