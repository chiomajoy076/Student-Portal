using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IAdminService
{
    Task<IEnumerable<StudentListViewModel>> GetStudentListAsync(string? search = null, string? status = null);
    Task<StudentDetailsViewModel?> GetStudentDetailsAsync(string id);
    Task<ServiceResult> ToggleActivationAsync(string id);
    Task<EditStudentViewModel?> GetEditStudentAsync(string id);
    Task<ServiceResult> UpdateStudentAsync(EditStudentViewModel model);
    Task<ServiceResult> ToggleLockAsync(string id);
    Task<ServiceResult> DeleteAccountAsync(string id);
    Task<List<UserAccountViewModel>> GetAllUsersAsync();
    Task<List<string>> GetAssignableRolesAsync();
    Task<ServiceResult> ChangeRoleAsync(string id, string newRole);
    Task<List<string>> GetStaffRolesAsync();
    Task<ServiceResult> CreateStaffAsync(CreateStaffViewModel model);
    Task<List<string>> GetDepartmentsForLecturerAsync(string id);
    Task<ServiceResult> SetLecturerDepartmentsAsync(string id, List<string> departments);
    Task<ServiceResult> SetExamOfficerForLecturerAsync(string id, bool enabled);
}
