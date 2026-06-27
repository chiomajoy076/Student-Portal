using System.Security.Claims;
using Student_Portal.Models;
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
    Task<ServiceResult> SaveFormAsync(ClaimsPrincipal principal, StudentFormViewModel model);
    Task<ServiceResult> UploadDocumentAsync(ClaimsPrincipal principal, IFormFile file);
    Task<List<AcademicPeriod>> GetAvailableResultPeriodsAsync(ClaimsPrincipal principal);
    Task<ResultCheckViewModel?> GetResultAsync(ClaimsPrincipal principal, string session, Semester semester);
    Task<byte[]?> GetRegistrationSlipAsync(ClaimsPrincipal principal);
}
