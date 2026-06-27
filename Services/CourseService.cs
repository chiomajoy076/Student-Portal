using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<List<CourseViewModel>> GetAllAsync(List<string>? allowedDepartments = null)
    {
        var courses = await _courseRepository.GetAllAsync();

        if (allowedDepartments != null)
        {
            courses = courses
                .Where(c => allowedDepartments.Contains(c.Department, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        return courses.Select(c => new CourseViewModel
        {
            Id = c.Id,
            CourseCode = c.CourseCode,
            CourseTitle = c.CourseTitle,
            CreditUnit = c.CreditUnit,
            Semester = c.Semester,
            Session = c.Session,
            Department = c.Department,
            Level = c.Level
        }).ToList();
    }

    public async Task<CourseViewModel?> GetByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            return null;
        }

        return new CourseViewModel
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseTitle = course.CourseTitle,
            CreditUnit = course.CreditUnit,
            Semester = course.Semester,
            Session = course.Session,
            Department = course.Department,
            Level = course.Level
        };
    }

    public async Task<ServiceResult> AddAsync(CourseViewModel model, List<string>? allowedDepartments = null)
    {
        if (allowedDepartments != null && !allowedDepartments.Contains(model.Department, StringComparer.OrdinalIgnoreCase))
        {
            return ServiceResult.Fail("You are not authorized to add courses for this department.");
        }

        var existing = await _courseRepository.FindAsync(model.CourseCode, model.Session, model.Semester);
        if (existing != null)
        {
            return ServiceResult.Fail("A course with this code already exists for the given session and semester.");
        }

        var course = new Course
        {
            CourseCode = model.CourseCode,
            CourseTitle = model.CourseTitle,
            CreditUnit = model.CreditUnit,
            Semester = model.Semester,
            Session = model.Session,
            Department = model.Department,
            Level = model.Level
        };

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAsync(CourseViewModel model)
    {
        var course = await _courseRepository.GetByIdAsync(model.Id);
        if (course == null)
        {
            return ServiceResult.Fail("Course not found.");
        }

        var existing = await _courseRepository.FindAsync(model.CourseCode, model.Session, model.Semester);
        if (existing != null && existing.Id != model.Id)
        {
            return ServiceResult.Fail("A course with this code already exists for the given session and semester.");
        }

        course.CourseCode = model.CourseCode;
        course.CourseTitle = model.CourseTitle;
        course.CreditUnit = model.CreditUnit;
        course.Semester = model.Semester;
        course.Session = model.Session;
        course.Department = model.Department;
        course.Level = model.Level;

        await _courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
