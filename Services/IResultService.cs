using Student_Portal.Models;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IResultService
{
    Task<ServiceResult> UploadSingleResultAsync(ResultUploadViewModel model, List<string>? allowedDepartments = null);
    Task<ServiceResult> UploadManyResultsAsync(BulkResultUploadViewModel model, List<string>? allowedDepartments = null);
    Task<(bool Found, string? Error, List<RegisteredCourseOptionViewModel> Courses)> GetFillableCoursesAsync(string matricNumber, List<string>? allowedDepartments = null);
    Task<ResultImportSummaryViewModel> ImportFromCsvAsync(IFormFile file, List<string>? allowedDepartments = null);
    Task<List<AcademicPeriod>> GetAvailablePeriodsAsync(string userId);
    Task<ResultCheckViewModel?> GetStudentResultAsync(string userId, string session, Semester semester);
}
