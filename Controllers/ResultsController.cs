using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Services;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

[Authorize(Roles = "SuperAdmin,Admin,ExamOfficer,Lecturer")]
public class ResultsController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IResultService _resultService;
    private readonly ILecturerService _lecturerService;

    public ResultsController(ICourseService courseService, IResultService resultService,
        ILecturerService lecturerService)
    {
        _courseService = courseService;
        _resultService = resultService;
        _lecturerService = lecturerService;
    }

    public async Task<IActionResult> Courses(int page = 1)
    {
        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var courses = await _courseService.GetPagedAsync(allowed, page);
        ViewBag.AllowedDepartments = allowed;
        return View(courses);
    }

    [HttpPost]
    public async Task<IActionResult> AddCourse(BulkCourseViewModel model)
    {
        if (!ModelState.IsValid || model.Rows.Count == 0)
        {
            TempData["Error"] = "Please fill in all required course fields correctly.";
            return RedirectToAction(nameof(Courses));
        }

        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var result = await _courseService.AddManyAsync(model, allowed);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? (model.Rows.Count == 1 ? "Course added successfully." : $"{model.Rows.Count} courses added successfully.")
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(Courses));
    }

    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet]
    public async Task<IActionResult> EditCourse(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost]
    public async Task<IActionResult> EditCourse(CourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _courseService.UpdateAsync(model);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Course updated successfully."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(Courses));
    }

    public IActionResult UploadResult()
    {
        return View(new BulkResultUploadViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> GetFillableCourses(string matricNumber)
    {
        if (string.IsNullOrWhiteSpace(matricNumber))
        {
            return Json(new { found = false, error = "Enter a matric number." });
        }

        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var (found, error, courses) = await _resultService.GetFillableCoursesAsync(matricNumber, allowed);

        if (!found)
        {
            return Json(new { found = false, error });
        }

        if (courses.Count == 0)
        {
            return Json(new { found = true, error = "This student has no registered courses awaiting results.", courses = Array.Empty<object>() });
        }

        return Json(new
        {
            found = true,
            error = (string?)null,
            courses = courses.Select(c => new
            {
                c.CourseId,
                c.CourseCode,
                c.CourseTitle,
                c.CreditUnit,
                c.Session,
                c.Semester
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> UploadResult(BulkResultUploadViewModel model)
    {
        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);

        if (!ModelState.IsValid || model.Rows.Count == 0)
        {
            TempData["Error"] = "Please select a matric number and enter at least one score.";
            return RedirectToAction(nameof(UploadResult));
        }

        var result = await _resultService.UploadManyResultsAsync(model, allowed);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? (model.Rows.Count == 1 ? "Result uploaded successfully." : $"{model.Rows.Count} results uploaded successfully.")
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(UploadResult));
    }

    public IActionResult ImportResults()
    {
        return View();
    }

    public IActionResult DownloadSampleCsv()
    {
        const string sample = "MatricNumber,CourseCode,Session,Semester,Score\n" +
                               "2024/CS/001,CSC101,2024/2025,First,75\n" +
                               "2024/CS/002,CSC101,2024/2025,First,68\n";

        var bytes = System.Text.Encoding.UTF8.GetBytes(sample);
        return File(bytes, "text/csv", "SampleResultUpload.csv");
    }

    [HttpPost]
    public async Task<IActionResult> ImportResults(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a CSV file to import.";
            return RedirectToAction(nameof(ImportResults));
        }

        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var summary = await _resultService.ImportFromCsvAsync(file, allowed);
        return View("ImportResultsSummary", summary);
    }

    public async Task<IActionResult> Roster(int page = 1)
    {
        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var roster = await _lecturerService.GetCourseRosterAsync(allowed, page);
        return View(roster);
    }
}
