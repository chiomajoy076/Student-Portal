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

    public async Task<IActionResult> Courses()
    {
        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var courses = await _courseService.GetAllAsync(allowed);
        ViewBag.AllowedDepartments = allowed;
        return View(courses);
    }

    [HttpPost]
    public async Task<IActionResult> AddCourse(CourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all required course fields correctly.";
            return RedirectToAction(nameof(Courses));
        }

        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var result = await _courseService.AddAsync(model, allowed);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Course added successfully."
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

    public async Task<IActionResult> UploadResult()
    {
        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        ViewBag.Courses = await _courseService.GetAllAsync(allowed);
        return View(new ResultUploadViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> UploadResult(ResultUploadViewModel model)
    {
        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);

        if (!ModelState.IsValid)
        {
            ViewBag.Courses = await _courseService.GetAllAsync(allowed);
            return View(model);
        }

        var result = await _resultService.UploadSingleResultAsync(model, allowed);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            ViewBag.Courses = await _courseService.GetAllAsync(allowed);
            return View(model);
        }

        TempData["Success"] = "Result uploaded successfully.";
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

    public async Task<IActionResult> Roster()
    {
        var allowed = await _lecturerService.GetAccessibleDepartmentsAsync(User);
        var roster = await _lecturerService.GetCourseRosterAsync(allowed);
        return View(roster);
    }
}
