using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IResultService
{
    Task<ServiceResult> UploadSingleResultAsync(ResultUploadViewModel model);
    Task<ResultImportSummaryViewModel> ImportFromCsvAsync(IFormFile file);
}
