using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Services;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<IActionResult> Index()
    {
        var status = await _studentService.GetFormStatusAsync(User);

        if (!status.Exists)
        {
            return RedirectToAction(nameof(Form));
        }

        return View(status.Form);
    }

    [HttpGet]
    public async Task<IActionResult> Form()
    {
        var status = await _studentService.GetFormStatusAsync(User);

        if (status.IsSubmitted)
        {
            // Redirect to Index if form is already submitted
            return RedirectToAction(nameof(Index));
        }

        return View(status.Form ?? new StudentFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Form(StudentFormViewModel model, IFormFile? document)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _studentService.SaveFormAsync(User, model, document);
        if (!result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = model.IsSubmitted
            ? "Your form has been submitted successfully."
            : "Your form has been saved.";

        return RedirectToAction(nameof(Index));
    }
}
