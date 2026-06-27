using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface IStudentRepository
{
    Task<StudentForm?> GetByUserIdAsync(string userId);
    Task<StudentForm?> GetByMatricNumberAsync(string matricNumber);
    Task<List<StudentForm>> GetAllAsync();
    Task AddAsync(StudentForm form);
    Task SaveChangesAsync();
}
