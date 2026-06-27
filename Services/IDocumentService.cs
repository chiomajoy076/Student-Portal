using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IDocumentService
{
    Task<DocumentViewModel?> GetLatestDocumentAsync(string userId);
    Task<ServiceResult> UploadAsync(string userId, IFormFile file);
}
