using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IAdminService
{
    Task<PagedResult<StudentListViewModel>> GetStudentListAsync(string? search = null, string? status = null, int page = 1, int pageSize = 20);
    Task<StudentDetailsViewModel?> GetStudentDetailsAsync(string id);
    Task<ServiceResult> ToggleActivationAsync(string id);
    Task<EditStudentViewModel?> GetEditStudentAsync(string id);
    Task<ServiceResult> UpdateStudentAsync(EditStudentViewModel model);
    Task<ServiceResult> ToggleLockAsync(string id);
    Task<ServiceResult> DeleteAccountAsync(string id);
    Task<PagedResult<UserAccountViewModel>> GetAllUsersAsync(int page = 1, int pageSize = 20);
    Task<List<string>> GetAssignableRolesAsync();
    Task<ServiceResult> ChangeRoleAsync(string id, string newRole);
    Task<List<string>> GetStaffRolesAsync();
    Task<ServiceResult> CreateStaffAsync(CreateStaffViewModel model);
    Task<EditStaffViewModel?> GetEditStaffAsync(string id);
    Task<ServiceResult> UpdateStaffAsync(EditStaffViewModel model);
}
