using System.Security.Claims;
using Student_Portal.Models;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface ICourseRegistrationService
{
    Task<List<AvailableCourseViewModel>> GetAvailableCoursesAsync(ClaimsPrincipal principal);
    Task<ServiceResult> RegisterAsync(ClaimsPrincipal principal, int courseId);
    Task<ServiceResult> UnregisterAsync(ClaimsPrincipal principal, int courseId);
    Task<ServiceResult> SubmitRegistrationAsync(ClaimsPrincipal principal, string session, Semester semester);
    Task<bool> IsRegisteredAsync(string userId, int courseId);

    Task<List<StudentRegisteredCourseViewModel>> GetRegisteredCoursesForAdminAsync(string studentUserId);
    Task<List<AvailableCourseViewModel>> GetEligibleCoursesForAdminAsync(string studentUserId);
    Task<ServiceResult> AdminRegisterAsync(string studentUserId, int courseId);
    Task<ServiceResult> AdminUnregisterAsync(string studentUserId, int courseId);
}
