using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Services;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

[Authorize(Roles = "SuperAdmin,Admin,ExamOfficer")]
public class ResultsController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IResultService _resultService;

    public ResultsController(ICourseService courseService, IResultService resultService)
    {
        _courseService = courseService;
        _resultService = resultService;
    }

    public async Task<IActionResult> Courses()
    {
        var courses = await _courseService.GetAllAsync();
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

        var result = await _courseService.AddAsync(model);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Course added successfully."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(Courses));
    }

    public async Task<IActionResult> UploadResult()
    {
        ViewBag.Courses = await _courseService.GetAllAsync();
        return View(new ResultUploadViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> UploadResult(ResultUploadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Courses = await _courseService.GetAllAsync();
            return View(model);
        }

        var result = await _resultService.UploadSingleResultAsync(model);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            ViewBag.Courses = await _courseService.GetAllAsync();
            return View(model);
        }

        TempData["Success"] = "Result uploaded successfully.";
        return RedirectToAction(nameof(UploadResult));
    }

    public IActionResult ImportResults()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ImportResults(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a CSV file to import.";
            return RedirectToAction(nameof(ImportResults));
        }

        var summary = await _resultService.ImportFromCsvAsync(file);
        return View("ImportResultsSummary", summary);
    }
}
