using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<StudentForm?> GetByUserIdAsync(string userId) =>
        _context.StudentForms.FirstOrDefaultAsync(f => f.UserId == userId);

    public Task<StudentForm?> GetByMatricNumberAsync(string matricNumber) =>
        _context.StudentForms.FirstOrDefaultAsync(f => f.MatricNumber == matricNumber);

    public Task<List<StudentForm>> GetAllAsync() => _context.StudentForms.ToListAsync();

    public async Task AddAsync(StudentForm form)
    {
        await _context.StudentForms.AddAsync(form);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
