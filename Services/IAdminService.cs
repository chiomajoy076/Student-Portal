using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IAdminService
{
    Task<IEnumerable<StudentListViewModel>> GetStudentListAsync();
    Task<StudentDetailsViewModel?> GetStudentDetailsAsync(string id);
    Task<ServiceResult> ToggleActivationAsync(string id);
}
