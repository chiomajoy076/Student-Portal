using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; }
}
