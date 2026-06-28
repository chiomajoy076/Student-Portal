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
    private readonly ICourseRegistrationService _courseRegistrationService;

    public AdminController(IAdminService adminService, IAuditService auditService,
        ICourseRegistrationService courseRegistrationService)
    {
        _adminService = adminService;
        _auditService = auditService;
        _courseRegistrationService = courseRegistrationService;
    }

    public async Task<IActionResult> Index(string? search, string? status, int page = 1)
    {
        var viewModel = await _adminService.GetStudentListAsync(search, status, page);
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
    public async Task<IActionResult> Users(int page = 1)
    {
        var users = await _adminService.GetAllUsersAsync(page);
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
    public async Task<IActionResult> AuditLogs(int page = 1, string? email = null)
    {
        var logs = await _auditService.GetPagedAsync(page, emailFilter: email);
        return View(logs);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> CreateStaff()
    {
        ViewBag.StaffRoles = await _adminService.GetStaffRolesAsync();
        return View(new CreateStaffViewModel());
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateStaff(CreateStaffViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.StaffRoles = await _adminService.GetStaffRolesAsync();
            return View(model);
        }

        var result = await _adminService.CreateStaffAsync(model);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            ViewBag.StaffRoles = await _adminService.GetStaffRolesAsync();
            return View(model);
        }

        TempData["Success"] = $"Staff account created successfully with role '{model.Role}'.";
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public async Task<IActionResult> StudentCourses(string id)
    {
        var registered = await _courseRegistrationService.GetRegisteredCoursesForAdminAsync(id);
        var eligible = await _courseRegistrationService.GetEligibleCoursesForAdminAsync(id);

        ViewBag.StudentId = id;
        ViewBag.EligibleCourses = eligible.Where(c => !c.IsRegistered).ToList();
        return View(registered);
    }

    [HttpPost]
    public async Task<IActionResult> AdminRegisterCourse(string id, int courseId)
    {
        var result = await _courseRegistrationService.AdminRegisterAsync(id, courseId);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Course registered for student."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(StudentCourses), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> AdminUnregisterCourse(string id, int courseId)
    {
        var result = await _courseRegistrationService.AdminUnregisterAsync(id, courseId);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Course unregistered for student."
            : string.Join(" ", result.Errors);

        return RedirectToAction(nameof(StudentCourses), new { id });
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> EditStaff(string id)
    {
        var model = await _adminService.GetEditStaffAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> EditStaff(EditStaffViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _adminService.UpdateStaffAsync(model);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Staff account updated successfully."
            : string.Join(" ", result.Errors);

        if (!result.Succeeded)
        {
            return View(model);
        }

        return RedirectToAction(nameof(Users));
    }
}
