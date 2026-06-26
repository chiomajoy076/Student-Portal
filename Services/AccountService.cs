using Microsoft.AspNetCore.Identity;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager)
    {
        _userRepository = userRepository;
        _signInManager = signInManager;
    }

    public async Task<ServiceResult> RegisterStudentAsync(StudentRegisterViewModel model)
    {
        var existing = await _userRepository.FindByEmailAsync(model.Email);
        if (existing != null)
        {
            return ServiceResult.Fail("Email Address already exist.");
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName,
            IsActive = false,
            PhoneNumber = model.PhoneNumber,
            Gender = model.Gender,
        };

        var result = await _userRepository.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(result.Errors.Select(e => e.Description));
        }

        await _userRepository.AddToRoleAsync(user, "Student");

        // Mail is not working - email confirmation disabled for now.

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> LoginAdminAsync(LoginViewModel model)
    {
        var user = await _userRepository.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return ServiceResult.Fail("Invalid login attempt.");
        }

        var isAdmin = await _userRepository.IsInRoleAsync(user, "Admin");
        var isSuperAdmin = await _userRepository.IsInRoleAsync(user, "SuperAdmin");

        if (!isAdmin && !isSuperAdmin)
        {
            return ServiceResult.Fail("You are not authorized to access admin area.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password,
            model.RememberMe, lockoutOnFailure: false);

        return signInResult.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Fail("Invalid login attempt.");
    }

    public async Task<ServiceResult> LoginStudentAsync(LoginViewModel model)
    {
        var user = await _userRepository.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return ServiceResult.Fail("Invalid login attempt One.");
        }

        var isStudent = await _userRepository.IsInRoleAsync(user, "Student");
        if (!isStudent)
        {
            return ServiceResult.Fail("This login is for students only.");
        }

        if (!user.IsActive)
        {
            return ServiceResult.Fail("Your account is not activated yet. Please wait for admin approval.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password,
            model.RememberMe, lockoutOnFailure: false);

        return signInResult.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Fail("Invalid login attempt.");
    }

    public Task LogoutAsync() => _signInManager.SignOutAsync();
}
