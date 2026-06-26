using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Services;

namespace Student_Portal.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = await _adminService.GetStudentListAsync();
        return View(viewModel);
    }

    public async Task<IActionResult> StudentDetails(string id)
    {
        var viewModel = await _adminService.GetStudentDetailsAsync(id);
        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActivation(string id)
    {
        var result = await _adminService.ToggleActivationAsync(id);
        if (!result.Succeeded)
        {
            TempData["Error"] = string.Join(" ", result.Errors);
            return NotFound();
        }

        TempData["Success"] = "Student account status updated.";
        return RedirectToAction(nameof(StudentDetails), new { id });
    }
}
