using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;

    public AdminService(IUserRepository userRepository, IStudentRepository studentRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<StudentListViewModel>> GetStudentListAsync()
    {
        var students = await _userRepository.GetUsersInRoleAsync("Student");
        var forms = await _studentRepository.GetAllAsync();

        return students.Select(s => new StudentListViewModel
        {
            Id = s.Id,
            Email = s.Email,
            FullName = $"{s.FirstName} {(string.IsNullOrWhiteSpace(s.MiddleName) ? "" : s.MiddleName)} {s.LastName}",
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            HasSubmittedForm = forms.Any(f => f.UserId == s.Id && f.IsSubmitted),
            Gender = s.Gender,
            PhoneNumber = s.PhoneNumber
        });
    }

    public async Task<StudentDetailsViewModel?> GetStudentDetailsAsync(string id)
    {
        var student = await _userRepository.FindByIdAsync(id);
        if (student == null)
        {
            return null;
        }

        var form = await _studentRepository.GetByUserIdAsync(id);

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
            Form = form != null ? new StudentFormViewModel
            {
                Id = form.Id,
                MatricNumber = form.MatricNumber,
                Department = form.Department,
                Level = form.Level,
                UploadedDocumentBase64 = form.UploadedDocument,
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

        return ServiceResult.Success();
    }
}
