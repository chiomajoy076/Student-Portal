using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IAccountService
{
    Task<ServiceResult> RegisterStudentAsync(StudentRegisterViewModel model);
    Task<LoginResult> LoginAdminAsync(LoginViewModel model);
    Task<ServiceResult> LoginStudentAsync(LoginViewModel model);
    Task LogoutAsync();
    Task<PasswordResetRequest> ForgotPasswordAsync(string email);
    Task<ServiceResult> ResetPasswordAsync(ResetPasswordViewModel model);
}
