using Student_Portal.Models;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IResultService
{
    Task<ServiceResult> UploadSingleResultAsync(ResultUploadViewModel model);
    Task<ResultImportSummaryViewModel> ImportFromCsvAsync(IFormFile file);
    Task<List<AcademicPeriod>> GetAvailablePeriodsAsync(string userId);
    Task<ResultCheckViewModel?> GetStudentResultAsync(string userId, string session, Semester semester);
}
