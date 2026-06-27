using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface ILecturerDepartmentRepository
{
    Task<List<LecturerDepartment>> GetByUserIdAsync(string userId);
    Task AddRangeAsync(IEnumerable<LecturerDepartment> items);
    void RemoveRange(IEnumerable<LecturerDepartment> items);
    Task SaveChangesAsync();
}
