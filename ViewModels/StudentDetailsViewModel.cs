using Student_Portal.Models;

namespace Student_Portal.ViewModels;

public class StudentDetailsViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public Gender? Gender { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public StudentFormViewModel Form { get; set; }
}