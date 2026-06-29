using System.Security.Claims;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface ILecturerService
{
    Task<List<string>> GetDepartmentsAsync(string userId);
    Task SetDepartmentsAsync(string userId, List<string> departments);
    Task<ServiceResult> SetExamOfficerAsync(string userId, bool enabled);

    /// <summary>
    /// Returns null when the user has unrestricted access (SuperAdmin, Admin, ExamOfficer),
    /// or the list of departments they're scoped to (Lecturer only, may be empty).
    /// </summary>
    Task<List<string>?> GetAccessibleDepartmentsAsync(ClaimsPrincipal principal);

    Task<PagedResult<CourseRosterViewModel>> GetCourseRosterAsync(List<string>? allowedDepartments, int page = 1, int pageSize = 10);
}
