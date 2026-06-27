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
    private readonly IRegistrationSlipService _registrationSlipService;

    public StudentService(IUserRepository userRepository, IStudentRepository studentRepository,
        IDocumentService documentService, IResultService resultService,
        IRegistrationSlipService registrationSlipService)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _documentService = documentService;
        _resultService = resultService;
        _registrationSlipService = registrationSlipService;
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

        // Matric number is never taken from user input - it's auto-assigned the first time the form is finally submitted.
        var matricNumber = form?.MatricNumber;
        if (model.IsSubmitted && string.IsNullOrEmpty(matricNumber))
        {
            matricNumber = await GenerateMatricNumberAsync();
        }

        if (form == null)
        {
            form = new StudentForm { UserId = user.Id };
            ApplyToEntity(form, model);
            form.MatricNumber = matricNumber;
            await _studentRepository.AddAsync(form);
        }
        else
        {
            ApplyToEntity(form, model);
            form.MatricNumber = matricNumber;
        }

        await _studentRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private async Task<string> GenerateMatricNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var allForms = await _studentRepository.GetAllAsync();
        var sequence = allForms.Count(f => !string.IsNullOrEmpty(f.MatricNumber)) + 1;

        string candidate;
        do
        {
            candidate = $"FUNAI/{year}/{sequence:0000}";
            sequence++;
        } while (await _studentRepository.GetByMatricNumberAsync(candidate) != null);

        return candidate;
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

    public async Task<byte[]?> GetRegistrationSlipAsync(ClaimsPrincipal principal)
    {
        var user = await _userRepository.GetUserAsync(principal);
        return await _registrationSlipService.GenerateAsync(user!.Id);
    }

    private static void ApplyToEntity(StudentForm form, StudentFormViewModel model)
    {
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
