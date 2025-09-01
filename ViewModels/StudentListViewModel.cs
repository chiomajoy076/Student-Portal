using Student_Portal.Models;

namespace Student_Portal.ViewModels;
public class StudentListViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasSubmittedForm { get; set; }
    public Gender? Gender { get; set; }
    public string PhoneNumber { get; set; }
}