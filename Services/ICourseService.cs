using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface ICourseService
{
    Task<List<CourseViewModel>> GetAllAsync(List<string>? allowedDepartments = null);
    Task<CourseViewModel?> GetByIdAsync(int id);
    Task<ServiceResult> AddAsync(CourseViewModel model, List<string>? allowedDepartments = null);
    Task<ServiceResult> UpdateAsync(CourseViewModel model);
}
