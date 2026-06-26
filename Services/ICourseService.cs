using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface ICourseService
{
    Task<List<CourseViewModel>> GetAllAsync();
    Task<ServiceResult> AddAsync(CourseViewModel model);
}
