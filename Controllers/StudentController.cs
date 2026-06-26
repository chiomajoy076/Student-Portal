using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Models;
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

        if (document != null)
        {
            var uploadResult = await _studentService.UploadDocumentAsync(User, document);
            if (!uploadResult.Succeeded)
            {
                foreach (var error in uploadResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }
        }

        var result = await _studentService.SaveFormAsync(User, model);
        if (!result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = document != null
            ? "Document uploaded and form saved successfully."
            : model.IsSubmitted
                ? "Your form has been submitted successfully."
                : "Your form has been saved.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> CheckResult(string? session, Semester? semester)
    {
        var periods = await _studentService.GetAvailableResultPeriodsAsync(User);
        ViewBag.Periods = periods;

        if (session == null || semester == null)
        {
            return View(null);
        }

        var result = await _studentService.GetResultAsync(User, session, semester.Value);
        if (result == null)
        {
            TempData["Error"] = "No result found for the selected session and semester.";
        }

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadRegistrationSlip()
    {
        var pdfBytes = await _studentService.GetRegistrationSlipAsync(User);
        if (pdfBytes == null)
        {
            TempData["Error"] = "Registration slip is only available after your form is submitted and your account is approved.";
            return RedirectToAction(nameof(Index));
        }

        return File(pdfBytes, "application/pdf", "RegistrationSlip.pdf");
    }
}
