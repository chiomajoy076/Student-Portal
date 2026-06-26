using System.Security.Claims;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class StudentFormStatus
{
    public StudentFormViewModel? Form { get; set; }
    public bool Exists { get; set; }
    public bool IsSubmitted { get; set; }
}

public interface IStudentService
{
    Task<StudentFormStatus> GetFormStatusAsync(ClaimsPrincipal principal);
    Task<ServiceResult> SaveFormAsync(ClaimsPrincipal principal, StudentFormViewModel model, IFormFile? document);
}
