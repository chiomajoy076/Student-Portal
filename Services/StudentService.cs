using System.Security.Claims;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class StudentService : IStudentService
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;

    public StudentService(IUserRepository userRepository, IStudentRepository studentRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
    }

    public async Task<StudentFormStatus> GetFormStatusAsync(ClaimsPrincipal principal)
    {
        var user = await _userRepository.GetUserAsync(principal);
        var form = await _studentRepository.GetByUserIdAsync(user!.Id);

        if (form == null)
        {
            return new StudentFormStatus { Exists = false, IsSubmitted = false, Form = null };
        }

        return new StudentFormStatus
        {
            Exists = true,
            IsSubmitted = form.IsSubmitted,
            Form = ToViewModel(form)
        };
    }

    public async Task<ServiceResult> SaveFormAsync(ClaimsPrincipal principal, StudentFormViewModel model, IFormFile? document)
    {
        var user = await _userRepository.GetUserAsync(principal);
        var form = await _studentRepository.GetByUserIdAsync(user!.Id);

        if (form?.IsSubmitted == true)
        {
            return ServiceResult.Fail("Form has already been submitted.");
        }

        if (document != null)
        {
            using var ms = new MemoryStream();
            await document.CopyToAsync(ms);
            model.UploadedDocumentBase64 = Convert.ToBase64String(ms.ToArray());
        }

        if (form == null)
        {
            form = new StudentForm { UserId = user.Id };
            ApplyToEntity(form, model, isNew: true);
            await _studentRepository.AddAsync(form);
        }
        else
        {
            ApplyToEntity(form, model, isNew: false);
        }

        await _studentRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static void ApplyToEntity(StudentForm form, StudentFormViewModel model, bool isNew)
    {
        form.MatricNumber = model.MatricNumber;
        form.Department = model.Department;
        form.Level = model.Level;

        if (isNew || model.UploadedDocumentBase64 != null)
        {
            form.UploadedDocument = model.UploadedDocumentBase64;
        }

        form.IsSubmitted = model.IsSubmitted;
        form.LastUpdated = DateTime.UtcNow;
        form.SubmittedAt = model.IsSubmitted ? DateTime.UtcNow : null;
        form.DateOfBirth = model.DateOfBirth;
        form.JambRegNumber = model.JambRegNumber;
        form.JambScore = model.JambScore;
        form.LocalGov = model.LocalGov;
        form.MaritalStatus = model.MaritalStatus;
        form.ModeOfEntry = model.ModeOfEntry;
        form.Nationality = model.Nationality;
        form.NextOfKin = model.NextOfKin;
        form.NextOfKinPhoneNumber = model.NextOfKinPhoneNumber;
        form.PostUtmeScore = model.PostUtmeScore;
        form.State = model.State;
        form.WAECRegNumber = model.WAECRegNumber;
    }

    private static StudentFormViewModel ToViewModel(StudentForm form) => new()
    {
        Id = form.Id,
        MatricNumber = form.MatricNumber,
        Department = form.Department,
        Level = form.Level,
        UploadedDocumentBase64 = form.UploadedDocument,
        IsSubmitted = form.IsSubmitted,
        LastUpdated = form.LastUpdated,
        SubmittedAt = form.SubmittedAt,
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
    };
}
