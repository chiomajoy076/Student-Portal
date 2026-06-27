using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Document?> GetLatestByUserIdAsync(string userId) =>
        _context.Documents
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.UploadDate)
            .FirstOrDefaultAsync();

    public async Task AddAsync(Document document)
    {
        await _context.Documents.AddAsync(document);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
