using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface IRegistrationSubmissionRepository
{
    Task<bool> ExistsAsync(string userId, string session, Semester semester);
    Task<List<RegistrationSubmission>> GetByUserIdAsync(string userId);
    Task AddAsync(RegistrationSubmission submission);
    Task SaveChangesAsync();
}
