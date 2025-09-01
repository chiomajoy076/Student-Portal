using Student_Portal.Models;
using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class StudentRegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string FirstName { get; set; }


    public string MiddleName { get; set; }

    [Required]
    public string LastName { get; set; }

    public Gender? Gender { get; set; }

    [Required]
    public string PhoneNumber { get; set; }
}

