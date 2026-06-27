namespace Student_Portal.ViewModels;

public class UserAccountViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string CurrentRole { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsLockedOut { get; set; }
    public bool IsLecturer { get; set; }
    public bool IsExamOfficer { get; set; }
    public List<string> Departments { get; set; } = new();
}
