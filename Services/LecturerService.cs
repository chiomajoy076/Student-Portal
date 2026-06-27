using System.Security.Claims;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class LecturerService : ILecturerService
{
    private readonly ILecturerDepartmentRepository _lecturerDepartmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseRegistrationRepository _courseRegistrationRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IAuditService _auditService;

    public LecturerService(ILecturerDepartmentRepository lecturerDepartmentRepository, IUserRepository userRepository,
        ICourseRepository courseRepository, ICourseRegistrationRepository courseRegistrationRepository,
        IStudentRepository studentRepository, IAuditService auditService)
    {
        _lecturerDepartmentRepository = lecturerDepartmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _studentRepository = studentRepository;
        _auditService = auditService;
    }

    public async Task<List<string>> GetDepartmentsAsync(string userId)
    {
        var departments = await _lecturerDepartmentRepository.GetByUserIdAsync(userId);
        return departments.Select(d => d.Department).ToList();
    }

    public async Task SetDepartmentsAsync(string userId, List<string> departments)
    {
        var existing = await _lecturerDepartmentRepository.GetByUserIdAsync(userId);
        _lecturerDepartmentRepository.RemoveRange(existing);

        await _lecturerDepartmentRepository.AddRangeAsync(departments.Select(d => new LecturerDepartment
        {
            UserId = userId,
            Department = d
        }));

        await _lecturerDepartmentRepository.SaveChangesAsync();
        await _auditService.LogAsync(userId, $"Lecturer departments set to: {string.Join(", ", departments)}");
    }

    public async Task<ServiceResult> SetExamOfficerAsync(string userId, bool enabled)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("Account not found.");
        }

        var roles = await _userRepository.GetRolesAsync(user);
        if (!roles.Contains("Lecturer"))
        {
            return ServiceResult.Fail("Only Lecturer accounts can be granted Exam Officer access this way.");
        }

        var isCurrentlyExamOfficer = roles.Contains("ExamOfficer");

        if (enabled && !isCurrentlyExamOfficer)
        {
            await _userRepository.AddToRoleAsync(user, "ExamOfficer");
            await _auditService.LogAsync(userId, "Granted ExamOfficer access (in addition to Lecturer role)");
        }
        else if (!enabled && isCurrentlyExamOfficer)
        {
            await _userRepository.RemoveFromRolesAsync(user, new[] { "ExamOfficer" });
            await _auditService.LogAsync(userId, "Revoked ExamOfficer access (Lecturer role retained)");
        }

        return ServiceResult.Success();
    }

    public async Task<List<string>?> GetAccessibleDepartmentsAsync(ClaimsPrincipal principal)
    {
        if (principal.IsInRole("SuperAdmin") || principal.IsInRole("Admin") || principal.IsInRole("ExamOfficer"))
        {
            return null;
        }

        if (principal.IsInRole("Lecturer"))
        {
            var user = await _userRepository.GetUserAsync(principal);
            return await GetDepartmentsAsync(user!.Id);
        }

        return new List<string>();
    }

    public async Task<List<CourseRosterViewModel>> GetCourseRosterAsync(List<string>? allowedDepartments)
    {
        var courses = await _courseRepository.GetAllAsync();
        if (allowedDepartments != null)
        {
            courses = courses
                .Where(c => allowedDepartments.Contains(c.Department, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        var courseIds = courses.Select(c => c.Id).ToList();
        var registrations = await _courseRegistrationRepository.GetByCourseIdsAsync(courseIds);
        var forms = await _studentRepository.GetAllAsync();

        return courses
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .Select(c => new CourseRosterViewModel
            {
                CourseId = c.Id,
                CourseCode = c.CourseCode,
                CourseTitle = c.CourseTitle,
                Department = c.Department,
                Session = c.Session,
                Semester = c.Semester,
                Students = registrations
                    .Where(r => r.CourseId == c.Id)
                    .Select(r => new RosterStudentRow
                    {
                        FullName = $"{r.User.FirstName} {r.User.LastName}",
                        MatricNumber = forms.FirstOrDefault(f => f.UserId == r.UserId)?.MatricNumber ?? "-",
                        Email = r.User.Email
                    })
                    .ToList()
            })
            .ToList();
    }
}
