using Microsoft.AspNetCore.Identity;

namespace Student_Portal.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public Gender? Gender { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ProfileImage { get; set; } // Base64 image

    /// <summary>
    /// Set when a SuperAdmin creates a staff account with a temporary password.
    /// The user is forced through the reset-password flow on first login before gaining access.
    /// </summary>
    public bool MustChangePassword { get; set; }
}

public enum Gender
{
    Male = 1,
    Female,
    Other
}

public enum MaritalStatus
{
    Single = 1,
    Married
}