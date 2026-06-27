using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IAccountService
{
    Task<ServiceResult> RegisterStudentAsync(StudentRegisterViewModel model);
    Task<ServiceResult> LoginAdminAsync(LoginViewModel model);
    Task<ServiceResult> LoginStudentAsync(LoginViewModel model);
    Task LogoutAsync();
}
