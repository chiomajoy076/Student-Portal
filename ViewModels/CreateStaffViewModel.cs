using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class CreateStaffViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; }

    [Display(Name = "Departments (Lecturer only)")]
    public List<string>? Departments { get; set; }

    [Display(Name = "Also grant Exam Officer access")]
    public bool AlsoExamOfficer { get; set; }
}
