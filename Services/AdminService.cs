using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class AdminService : IAdminService
{
    private static readonly string[] AssignableRoles = { "Admin", "ExamOfficer", "Student" };

    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IDocumentService _documentService;
    private readonly IAuditService _auditService;

    public AdminService(IUserRepository userRepository, IStudentRepository studentRepository,
        IDocumentService documentService, IAuditService auditService)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _documentService = documentService;
        _auditService = auditService;
    }

    public async Task<IEnumerable<StudentListViewModel>> GetStudentListAsync(string? search = null, string? status = null)
    {
        var students = await _userRepository.GetUsersInRoleAsync("Student");
        var forms = await _studentRepository.GetAllAsync();

        var list = students.Select(s => new StudentListViewModel
        {
            Id = s.Id,
            Email = s.Email,
            FullName = $"{s.FirstName} {(string.IsNullOrWhiteSpace(s.MiddleName) ? "" : s.MiddleName)} {s.LastName}",
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            HasSubmittedForm = forms.Any(f => f.UserId == s.Id && f.IsSubmitted),
            Gender = s.Gender,
            PhoneNumber = s.PhoneNumber,
            MatricNumber = forms.FirstOrDefault(f => f.UserId == s.Id)?.MatricNumber,
            Department = forms.FirstOrDefault(f => f.UserId == s.Id)?.Department
        });

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            list = list.Where(s =>
                (s.FullName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.MatricNumber?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.Department?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
        {
            list = list.Where(s => s.IsActive);
        }
        else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
        {
            list = list.Where(s => !s.IsActive);
        }

        return list.ToList();
    }

    public async Task<StudentDetailsViewModel?> GetStudentDetailsAsync(string id)
    {
        var student = await _userRepository.FindByIdAsync(id);
        if (student == null)
        {
            return null;
        }

        var form = await _studentRepository.GetByUserIdAsync(id);
        var document = await _documentService.GetLatestDocumentAsync(id);

        return new StudentDetailsViewModel
        {
            Id = student.Id,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            MiddleName = student.MiddleName,
            IsActive = student.IsActive,
            PhoneNumber = student.PhoneNumber,
            Gender = student.Gender,
            IsLockedOut = await _userRepository.IsLockedOutAsync(student),
            Form = form != null ? new StudentFormViewModel
            {
                Id = form.Id,
                MatricNumber = form.MatricNumber,
                Department = form.Department,
                Level = form.Level,
                DocumentFileName = document?.FileName,
                DocumentUrl = document?.Url,
                IsSubmitted = form.IsSubmitted,
                LastUpdated = form.LastUpdated,
                DateOfBirth = form.DateOfBirth,
                JambRegNumber = form.JambRegNumber,
                JambScore = form.JambScore,
                LocalGov = form.LocalGov,
                MaritalStatus = form.MaritalStatus,
                ModeOfEntry = form.ModeOfEntry,
                Nationality = form.Nationality,
                NextOfKin = form.NextOfKin,
                NextOfKinPhoneNumber = form.NextOfKinPhoneNumber,
                PostUtmeScore = form.PostUtmeScore,
                State = form.State,
                WAECRegNumber = form.WAECRegNumber,
            } : null
        };
    }

    public async Task<ServiceResult> ToggleActivationAsync(string id)
    {
        var student = await _userRepository.FindByIdAsync(id);
        if (student == null)
        {
            return ServiceResult.Fail("Student not found.");
        }

        student.IsActive = !student.IsActive;
        student.EmailConfirmed = !student.EmailConfirmed;
        await _userRepository.UpdateAsync(student);

        await _auditService.LogAsync(id, student.IsActive ? "Student account approved/activated" : "Student account deactivated");

        return ServiceResult.Success();
    }

    public async Task<EditStudentViewModel?> GetEditStudentAsync(string id)
    {
        var student = await _userRepository.FindByIdAsync(id);
        if (student == null)
        {
            return null;
        }

        var form = await _studentRepository.GetByUserIdAsync(id);

        return new EditStudentViewModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            MiddleName = student.MiddleName,
            LastName = student.LastName,
            PhoneNumber = student.PhoneNumber,
            Gender = student.Gender,
            MatricNumber = form?.MatricNumber,
            Department = form?.Department,
            Level = form?.Level
        };
    }

    public async Task<ServiceResult> UpdateStudentAsync(EditStudentViewModel model)
    {
        var student = await _userRepository.FindByIdAsync(model.Id);
        if (student == null)
        {
            return ServiceResult.Fail("Student not found.");
        }

        student.FirstName = model.FirstName;
        student.MiddleName = model.MiddleName;
        student.LastName = model.LastName;
        student.PhoneNumber = model.PhoneNumber;
        student.Gender = model.Gender;
        await _userRepository.UpdateAsync(student);

        var form = await _studentRepository.GetByUserIdAsync(model.Id);
        if (form != null)
        {
            form.MatricNumber = model.MatricNumber;
            form.Department = model.Department;
            form.Level = model.Level;
            await _studentRepository.SaveChangesAsync();
        }

        await _auditService.LogAsync(model.Id, "Student record edited by admin");

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> ToggleLockAsync(string id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Fail("Account not found.");
        }

        var isLocked = await _userRepository.IsLockedOutAsync(user);
        if (isLocked)
        {
            await _userRepository.SetLockoutEndDateAsync(user, null);
            await _auditService.LogAsync(id, "Account unlocked by admin");
        }
        else
        {
            await _userRepository.SetLockoutEnabledAsync(user, true);
            await _userRepository.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            await _auditService.LogAsync(id, "Account locked by admin");
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAccountAsync(string id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Fail("Account not found.");
        }

        var result = await _userRepository.DeleteAsync(user);
        if (result.Succeeded)
        {
            await _auditService.LogAsync(null, $"Account deleted by admin (was: {user.Email})");
            return ServiceResult.Success();
        }

        return ServiceResult.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<List<UserAccountViewModel>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        var result = new List<UserAccountViewModel>();

        foreach (var user in users)
        {
            var roles = await _userRepository.GetRolesAsync(user);
            result.Add(new UserAccountViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                CurrentRole = roles.FirstOrDefault() ?? "(none)",
                IsLockedOut = await _userRepository.IsLockedOutAsync(user)
            });
        }

        return result;
    }

    public Task<List<string>> GetAssignableRolesAsync() => Task.FromResult(AssignableRoles.ToList());

    public async Task<ServiceResult> ChangeRoleAsync(string id, string newRole)
    {
        if (!AssignableRoles.Contains(newRole))
        {
            return ServiceResult.Fail("Invalid role selected.");
        }

        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Fail("Account not found.");
        }

        var currentRoles = await _userRepository.GetRolesAsync(user);
        if (currentRoles.Contains("SuperAdmin"))
        {
            return ServiceResult.Fail("Cannot change the role of a Super Admin account.");
        }

        await _userRepository.RemoveFromRolesAsync(user, currentRoles);
        await _userRepository.AddToRoleAsync(user, newRole);

        await _auditService.LogAsync(id, $"Role changed to '{newRole}' by SuperAdmin");

        return ServiceResult.Success();
    }
}
