using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface ICourseService
{
    Task<List<CourseViewModel>> GetAllAsync(List<string>? allowedDepartments = null);
    Task<PagedResult<CourseViewModel>> GetPagedAsync(List<string>? allowedDepartments = null, int page = 1, int pageSize = 20);
    Task<CourseViewModel?> GetByIdAsync(int id);
    Task<ServiceResult> AddAsync(CourseViewModel model, List<string>? allowedDepartments = null);
    Task<ServiceResult> AddManyAsync(BulkCourseViewModel model, List<string>? allowedDepartments = null);
    Task<ServiceResult> UpdateAsync(CourseViewModel model);
}
