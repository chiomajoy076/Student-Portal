using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class DocumentService : IDocumentService
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private readonly IDocumentRepository _documentRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DocumentService(IDocumentRepository documentRepository, IWebHostEnvironment webHostEnvironment)
    {
        _documentRepository = documentRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<DocumentViewModel?> GetLatestDocumentAsync(string userId)
    {
        var document = await _documentRepository.GetLatestByUserIdAsync(userId);
        if (document == null)
        {
            return null;
        }

        return new DocumentViewModel
        {
            FileName = document.FileName,
            Url = document.FilePath,
            UploadDate = document.UploadDate
        };
    }

    public async Task<ServiceResult> UploadAsync(string userId, IFormFile file)
    {
        if (file.Length == 0)
        {
            return ServiceResult.Fail("The selected file is empty.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return ServiceResult.Fail("File size must not exceed 5MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return ServiceResult.Fail("Only PDF, JPG, and PNG files are allowed.");
        }

        var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", userId);
        Directory.CreateDirectory(uploadsRoot);

        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadsRoot, storedFileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var document = new Document
        {
            UserId = userId,
            FileName = file.FileName,
            FilePath = $"/uploads/{userId}/{storedFileName}",
            FileType = extension,
            UploadDate = DateTime.UtcNow
        };

        await _documentRepository.AddAsync(document);
        await _documentRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
