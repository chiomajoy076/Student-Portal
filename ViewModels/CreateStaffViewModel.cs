using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class CreateStaffViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; }

    [Display(Name = "Departments (Lecturer only)")]
    public List<string>? Departments { get; set; }

    [Display(Name = "Also grant Exam Officer access")]
    public bool AlsoExamOfficer { get; set; }
}
