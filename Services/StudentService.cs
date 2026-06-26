using System.Security.Claims;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class StudentService : IStudentService
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IDocumentService _documentService;
    private readonly IResultService _resultService;

    public StudentService(IUserRepository userRepository, IStudentRepository studentRepository,
        IDocumentService documentService, IResultService resultService)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _documentService = documentService;
        _resultService = resultService;
    }

    public async Task<StudentFormStatus> GetFormStatusAsync(ClaimsPrincipal principal)
    {
        var user = await _userRepository.GetUserAsync(principal);
        var form = await _studentRepository.GetByUserIdAsync(user!.Id);
        var document = await _documentService.GetLatestDocumentAsync(user.Id);

        if (form == null)
        {
            return new StudentFormStatus
            {
                Exists = false,
                IsSubmitted = false,
                Form = ApplyDocument(new StudentFormViewModel(), document)
            };
        }

        return new StudentFormStatus
        {
            Exists = true,
            IsSubmitted = form.IsSubmitted,
            Form = ApplyDocument(ToViewModel(form), document)
        };
    }

    public async Task<ServiceResult> SaveFormAsync(ClaimsPrincipal principal, StudentFormViewModel model)
    {
        var user = await _userRepository.GetUserAsync(principal);
        var form = await _studentRepository.GetByUserIdAsync(user!.Id);

        if (form?.IsSubmitted == true)
        {
            return ServiceResult.Fail("Form has already been submitted.");
        }

        if (form == null)
        {
            form = new StudentForm { UserId = user.Id };
            ApplyToEntity(form, model);
            await _studentRepository.AddAsync(form);
        }
        else
        {
            ApplyToEntity(form, model);
        }

        await _studentRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UploadDocumentAsync(ClaimsPrincipal principal, IFormFile file)
    {
        var user = await _userRepository.GetUserAsync(principal);
        return await _documentService.UploadAsync(user!.Id, file);
    }

    public async Task<List<AcademicPeriod>> GetAvailableResultPeriodsAsync(ClaimsPrincipal principal)
    {
        var user = await _userRepository.GetUserAsync(principal);
        return await _resultService.GetAvailablePeriodsAsync(user!.Id);
    }

    public async Task<ResultCheckViewModel?> GetResultAsync(ClaimsPrincipal principal, string session, Semester semester)
    {
        var user = await _userRepository.GetUserAsync(principal);
        return await _resultService.GetStudentResultAsync(user!.Id, session, semester);
    }

    private static void ApplyToEntity(StudentForm form, StudentFormViewModel model)
    {
        form.MatricNumber = model.MatricNumber;
        form.Department = model.Department;
        form.Level = model.Level;
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

    private static StudentFormViewModel ApplyDocument(StudentFormViewModel viewModel, DocumentViewModel? document)
    {
        if (document != null)
        {
            viewModel.DocumentFileName = document.FileName;
            viewModel.DocumentUrl = document.Url;
        }

        return viewModel;
    }

    private static StudentFormViewModel ToViewModel(StudentForm form) => new()
    {
        Id = form.Id,
        MatricNumber = form.MatricNumber,
        Department = form.Department,
        Level = form.Level,
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
