using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface IDocumentRepository
{
    Task<Document?> GetLatestByUserIdAsync(string userId);
    Task AddAsync(Document document);
    Task SaveChangesAsync();
}
