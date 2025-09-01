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