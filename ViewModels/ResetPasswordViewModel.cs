using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }

    public string Context { get; set; } = "student";

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match.")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; }
}
