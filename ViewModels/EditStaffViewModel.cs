using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class EditStaffViewModel
{
    public string Id { get; set; }

    public string Email { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }

    public string CurrentRole { get; set; }

    public bool IsLecturer { get; set; }

    [Display(Name = "Also grant Exam Officer access")]
    public bool AlsoExamOfficer { get; set; }

    [Display(Name = "Departments")]
    public List<string> Departments { get; set; } = new();
}
