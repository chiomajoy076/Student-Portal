using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class LecturerDepartmentRepository : ILecturerDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public LecturerDepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<LecturerDepartment>> GetByUserIdAsync(string userId) =>
        _context.LecturerDepartments.Where(ld => ld.UserId == userId).ToListAsync();

    public async Task AddRangeAsync(IEnumerable<LecturerDepartment> items)
    {
        await _context.LecturerDepartments.AddRangeAsync(items);
    }

    public void RemoveRange(IEnumerable<LecturerDepartment> items)
    {
        _context.LecturerDepartments.RemoveRange(items);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
