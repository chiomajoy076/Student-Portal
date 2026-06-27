using System.Security.Claims;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class CourseRegistrationService : ICourseRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseRegistrationRepository _registrationRepository;
    private readonly IRegistrationSubmissionRepository _submissionRepository;
    private readonly IResultRepository _resultRepository;
    private readonly IAuditService _auditService;

    public CourseRegistrationService(IUserRepository userRepository, IStudentRepository studentRepository,
        ICourseRepository courseRepository, ICourseRegistrationRepository registrationRepository,
        IRegistrationSubmissionRepository submissionRepository, IResultRepository resultRepository,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _registrationRepository = registrationRepository;
        _submissionRepository = submissionRepository;
        _resultRepository = resultRepository;
        _auditService = auditService;
    }

    public async Task<List<AvailableCourseViewModel>> GetAvailableCoursesAsync(ClaimsPrincipal principal)
    {
        var user = await _userRepository.GetUserAsync(principal);
        return await BuildAvailableCoursesAsync(user!.Id);
    }

    public Task<List<AvailableCourseViewModel>> GetEligibleCoursesForAdminAsync(string studentUserId) =>
        BuildAvailableCoursesAsync(studentUserId);

    private async Task<List<AvailableCourseViewModel>> BuildAvailableCoursesAsync(string userId)
    {
        var form = await _studentRepository.GetByUserIdAsync(userId);
        if (form == null)
        {
            return new List<AvailableCourseViewModel>();
        }

        var allCourses = await _courseRepository.GetAllAsync();
        var registrations = await _registrationRepository.GetByUserIdAsync(userId);
        var registeredCourseIds = registrations.Select(r => r.CourseId).ToHashSet();
        var submissions = await _submissionRepository.GetByUserIdAsync(userId);
        var finalizedPeriods = submissions.Select(s => (s.Session, s.Semester)).ToHashSet();

        return allCourses
            .Where(c => string.Equals(c.Department, form.Department, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(c.Level, form.Level, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Session)
            .ThenBy(c => c.Semester)
            .ThenBy(c => c.CourseCode)
            .Select(c => new AvailableCourseViewModel
            {
                CourseId = c.Id,
                CourseCode = c.CourseCode,
                CourseTitle = c.CourseTitle,
                CreditUnit = c.CreditUnit,
                Session = c.Session,
                Semester = c.Semester,
                IsRegistered = registeredCourseIds.Contains(c.Id),
                IsFinalized = finalizedPeriods.Contains((c.Session, c.Semester))
            })
            .ToList();
    }

    public async Task<ServiceResult> RegisterAsync(ClaimsPrincipal principal, int courseId)
    {
        var user = await _userRepository.GetUserAsync(principal);
        return await RegisterInternalAsync(user!.Id, courseId, enforceFinalizedLock: true);
    }

    public Task<ServiceResult> AdminRegisterAsync(string studentUserId, int courseId) =>
        RegisterInternalAsync(studentUserId, courseId, enforceFinalizedLock: false);

    private async Task<ServiceResult> RegisterInternalAsync(string studentUserId, int courseId, bool enforceFinalizedLock)
    {
        var form = await _studentRepository.GetByUserIdAsync(studentUserId);
        if (form == null)
        {
            return ServiceResult.Fail("Complete the biodata form before registering for courses.");
        }

        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return ServiceResult.Fail("Course not found.");
        }

        if (!string.Equals(course.Department, form.Department, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(course.Level, form.Level, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult.Fail("This student is not eligible to register for this course.");
        }

        if (enforceFinalizedLock && await _submissionRepository.ExistsAsync(studentUserId, course.Session, course.Semester))
        {
            return ServiceResult.Fail("Registration has been finalized for this session/semester. Contact an admin to make changes.");
        }

        if (await _registrationRepository.ExistsAsync(studentUserId, courseId))
        {
            return ServiceResult.Fail("Already registered for this course.");
        }

        await _registrationRepository.AddAsync(new CourseRegistration
        {
            UserId = studentUserId,
            CourseId = courseId,
            RegisteredAt = DateTime.UtcNow
        });
        await _registrationRepository.SaveChangesAsync();

        await _auditService.LogAsync(studentUserId, $"Registered for course '{course.CourseCode}'");

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UnregisterAsync(ClaimsPrincipal principal, int courseId)
    {
        var user = await _userRepository.GetUserAsync(principal);
        return await UnregisterInternalAsync(user!.Id, courseId, enforceFinalizedLock: true);
    }

    public Task<ServiceResult> AdminUnregisterAsync(string studentUserId, int courseId) =>
        UnregisterInternalAsync(studentUserId, courseId, enforceFinalizedLock: false);

    private async Task<ServiceResult> UnregisterInternalAsync(string studentUserId, int courseId, bool enforceFinalizedLock)
    {
        var registration = await _registrationRepository.GetAsync(studentUserId, courseId);
        if (registration == null)
        {
            return ServiceResult.Fail("Not registered for this course.");
        }

        if (enforceFinalizedLock &&
            await _submissionRepository.ExistsAsync(studentUserId, registration.Course?.Session ?? "", registration.Course?.Semester ?? Semester.First))
        {
            return ServiceResult.Fail("Registration has been finalized for this session/semester. Contact an admin to make changes.");
        }

        if (await _resultRepository.ExistsAsync(studentUserId, courseId))
        {
            return ServiceResult.Fail("Cannot unregister; a result has already been recorded for this course.");
        }

        _registrationRepository.Remove(registration);
        await _registrationRepository.SaveChangesAsync();

        await _auditService.LogAsync(studentUserId, $"Unregistered from course id {courseId}");

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SubmitRegistrationAsync(ClaimsPrincipal principal, string session, Semester semester)
    {
        var user = await _userRepository.GetUserAsync(principal);

        if (await _submissionRepository.ExistsAsync(user!.Id, session, semester))
        {
            return ServiceResult.Fail("Registration has already been finalized for this session/semester.");
        }

        var registrations = await _registrationRepository.GetByUserIdAsync(user.Id);
        var hasCoursesInPeriod = registrations.Any(r => r.Course.Session == session && r.Course.Semester == semester);
        if (!hasCoursesInPeriod)
        {
            return ServiceResult.Fail("Register for at least one course before submitting.");
        }

        await _submissionRepository.AddAsync(new RegistrationSubmission
        {
            UserId = user.Id,
            Session = session,
            Semester = semester,
            SubmittedAt = DateTime.UtcNow
        });
        await _submissionRepository.SaveChangesAsync();

        await _auditService.LogAsync(user.Id, $"Finalized course registration for {session} {semester} semester");

        return ServiceResult.Success();
    }

    public Task<bool> IsRegisteredAsync(string userId, int courseId) =>
        _registrationRepository.ExistsAsync(userId, courseId);

    public async Task<List<StudentRegisteredCourseViewModel>> GetRegisteredCoursesForAdminAsync(string studentUserId)
    {
        var registrations = await _registrationRepository.GetByUserIdAsync(studentUserId);

        return registrations.Select(r => new StudentRegisteredCourseViewModel
        {
            CourseId = r.CourseId,
            CourseCode = r.Course.CourseCode,
            CourseTitle = r.Course.CourseTitle,
            Session = r.Course.Session,
            Semester = r.Course.Semester
        }).ToList();
    }
}
