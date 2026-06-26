namespace Student_Portal.ViewModels;

public class UserAccountViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string CurrentRole { get; set; }
    public bool IsLockedOut { get; set; }
}
