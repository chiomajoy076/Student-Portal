using Student_Portal.Models;
using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class StudentRegisterViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Middle Name")]
    public string MiddleName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Display(Name = "Gender")]
    public Gender? Gender { get; set; }

    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
}

