using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Services;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IAuditService _auditService;

    public AdminController(IAdminService adminService, IAuditService auditService)
    {
        _adminService = adminService;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index(string? search, string? status)
    {
        var viewModel = await _adminService.GetStudentListAsync(search, status);
        ViewBag.Search = search;
        ViewBag.Status = status;
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

    [HttpGet]
    public async Task<IActionResult> EditStudent(string id)
    {
        var model = await _adminService.GetEditStudentAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditStudent(EditStudentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _adminService.UpdateStudentAsync(model);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Student record updated successfully."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(StudentDetails), new { id = model.Id });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var result = await _adminService.ToggleLockAsync(id);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Account lock status updated."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(StudentDetails), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(string id)
    {
        var result = await _adminService.DeleteAccountAsync(id);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Account deleted successfully."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Users()
    {
        var users = await _adminService.GetAllUsersAsync();
        ViewBag.AssignableRoles = await _adminService.GetAssignableRolesAsync();
        return View(users);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string id, string newRole)
    {
        var result = await _adminService.ChangeRoleAsync(id, newRole);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Role updated successfully."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(Users));
    }

    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AuditLogs()
    {
        var logs = await _auditService.GetRecentAsync();
        return View(logs);
    }
}
