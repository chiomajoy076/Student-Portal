using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var students = await _userManager.GetUsersInRoleAsync("Student");
        var forms = await _context.StudentForms.ToListAsync();

        var viewModel = students.Select(s => new StudentListViewModel
        {
            Id = s.Id,
            Email = s.Email,
            FullName = $"{s.FirstName} {(string.IsNullOrWhiteSpace(s.MiddleName) ? "" : s.MiddleName)} {s.LastName}",
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            HasSubmittedForm = forms.Any(f => f.UserId == s.Id && f.IsSubmitted),
            Gender = s.Gender,
            PhoneNumber = s.PhoneNumber
        });

        return View(viewModel);
    }

    public async Task<IActionResult> StudentDetails(string id)
    {
        var student = await _userManager.FindByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var form = await _context.StudentForms
            .FirstOrDefaultAsync(f => f.UserId == id);

        var viewModel = new StudentDetailsViewModel
        {
            Id = student.Id,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            MiddleName = student.MiddleName,
            IsActive = student.IsActive,
            PhoneNumber = student.PhoneNumber,
            Gender = student.Gender,
            
            Form = form != null ? new StudentFormViewModel
            {
                Id = form.Id,
                MatricNumber = form.MatricNumber,
                Department = form.Department,
                Level = form.Level,
                UploadedDocumentBase64 = form.UploadedDocument,
                IsSubmitted = form.IsSubmitted,
                LastUpdated = form.LastUpdated,
                DateOfBirth = form.DateOfBirth,
                JambRegNumber = form.JambRegNumber,
                JambScore = form.JambScore,
                LocalGov = form.LocalGov,
                MaritalStatus = form.MaritalStatus,
                ModeOfEntry = form.ModeOfEntry,
                Nationality = form.Nationality,
                NextOfKin = form.NextOfKin,
                NextOfKinPhoneNumber = form.NextOfKinPhoneNumber,
                PostUtmeScore = form.PostUtmeScore,
                State = form.State,
                WAECRegNumber = form.WAECRegNumber,
            } : null
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActivation(string id)
    {
        var student = await _userManager.FindByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        student.IsActive = !student.IsActive;
        student.EmailConfirmed = !student.EmailConfirmed; 
        await _userManager.UpdateAsync(student);

        return RedirectToAction(nameof(StudentDetails), new { id });
    }
}