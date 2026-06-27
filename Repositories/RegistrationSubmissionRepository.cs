using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class RegistrationSubmissionRepository : IRegistrationSubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public RegistrationSubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(string userId, string session, Semester semester) =>
        _context.RegistrationSubmissions.AnyAsync(rs =>
            rs.UserId == userId && rs.Session == session && rs.Semester == semester);

    public Task<List<RegistrationSubmission>> GetByUserIdAsync(string userId) =>
        _context.RegistrationSubmissions.Where(rs => rs.UserId == userId).ToListAsync();

    public async Task AddAsync(RegistrationSubmission submission)
    {
        await _context.RegistrationSubmissions.AddAsync(submission);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
