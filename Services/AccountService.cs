using Microsoft.AspNetCore.Identity;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuditService _auditService;

    public AccountService(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _signInManager = signInManager;
        _auditService = auditService;
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
        await _auditService.LogAsync(user.Id, "Student registered (pending admin approval)");

        // Mail is not working - email confirmation disabled for now.

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> LoginAdminAsync(LoginViewModel model)
    {
        var user = await _userRepository.FindByEmailAsync(model.Email);
        if (user == null)
        {
            await _auditService.LogAsync(null, $"Failed admin login attempt for unknown email '{model.Email}'");
            return ServiceResult.Fail("Invalid login attempt.");
        }

        var isAdmin = await _userRepository.IsInRoleAsync(user, "Admin");
        var isSuperAdmin = await _userRepository.IsInRoleAsync(user, "SuperAdmin");
        var isExamOfficer = await _userRepository.IsInRoleAsync(user, "ExamOfficer");
        var isLecturer = await _userRepository.IsInRoleAsync(user, "Lecturer");

        if (!isAdmin && !isSuperAdmin && !isExamOfficer && !isLecturer)
        {
            await _auditService.LogAsync(user.Id, "Attempted admin login without admin role");
            return ServiceResult.Fail("You are not authorized to access admin area.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password,
            model.RememberMe, lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            await _auditService.LogAsync(user.Id, "Admin login succeeded");
            return ServiceResult.Success();
        }

        if (signInResult.IsLockedOut)
        {
            await _auditService.LogAsync(user.Id, "Admin account locked out after repeated failed login attempts");
            return ServiceResult.Fail("This account has been temporarily locked due to multiple failed login attempts. Try again later.");
        }

        await _auditService.LogAsync(user.Id, "Failed admin login attempt (wrong password)");
        return ServiceResult.Fail("Invalid login attempt.");
    }

    public async Task<ServiceResult> LoginStudentAsync(LoginViewModel model)
    {
        var user = await _userRepository.FindByEmailAsync(model.Email);
        if (user == null)
        {
            await _auditService.LogAsync(null, $"Failed student login attempt for unknown email '{model.Email}'");
            return ServiceResult.Fail("Invalid login attempt One.");
        }

        var isStudent = await _userRepository.IsInRoleAsync(user, "Student");
        if (!isStudent)
        {
            await _auditService.LogAsync(user.Id, "Attempted student login without student role");
            return ServiceResult.Fail("This login is for students only.");
        }

        if (!user.IsActive)
        {
            await _auditService.LogAsync(user.Id, "Login blocked: account pending admin approval");
            return ServiceResult.Fail("Your account is not activated yet. Please wait for admin approval.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password,
            model.RememberMe, lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            await _auditService.LogAsync(user.Id, "Student login succeeded");
            return ServiceResult.Success();
        }

        if (signInResult.IsLockedOut)
        {
            await _auditService.LogAsync(user.Id, "Student account locked out after repeated failed login attempts");
            return ServiceResult.Fail("This account has been temporarily locked due to multiple failed login attempts. Try again later.");
        }

        await _auditService.LogAsync(user.Id, "Failed student login attempt (wrong password)");
        return ServiceResult.Fail("Invalid login attempt.");
    }

    public async Task LogoutAsync()
    {
        var userId = _signInManager.Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _signInManager.SignOutAsync();
        await _auditService.LogAsync(userId, "Logged out");
    }
}
