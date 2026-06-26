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

    public async Task<List<CourseViewModel>> GetAllAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        return courses.Select(c => new CourseViewModel
        {
            Id = c.Id,
            CourseCode = c.CourseCode,
            CourseTitle = c.CourseTitle,
            CreditUnit = c.CreditUnit,
            Semester = c.Semester,
            Session = c.Session,
            Department = c.Department
        }).ToList();
    }

    public async Task<ServiceResult> AddAsync(CourseViewModel model)
    {
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
            Department = model.Department
        };

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
