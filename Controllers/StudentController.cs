using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Portal.Data;
using Student_Portal.Models;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public StudentController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var form = await _context.StudentForms
            .FirstOrDefaultAsync(f => f.UserId == user.Id);

        if (form == null)
        {
            return RedirectToAction(nameof(Form));
        }

        var viewModel = new StudentFormViewModel
        {
            Id = form.Id,
            MatricNumber = form.MatricNumber,
            Department = form.Department,
            Level = form.Level,
            UploadedDocumentBase64 = form.UploadedDocument,
            IsSubmitted = form.IsSubmitted,
            LastUpdated = form.LastUpdated,
            SubmittedAt = form.SubmittedAt,
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
        };

        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> Form()
    {
        var user = await _userManager.GetUserAsync(User);
        var form = await _context.StudentForms
            .FirstOrDefaultAsync(f => f.UserId == user.Id);

        if (form != null && form.IsSubmitted)
        {
            // Redirect to Index if form is already submitted
            return RedirectToAction(nameof(Index));
        }

        if (form == null)
        {
            return View(new StudentFormViewModel());
        }

        var viewModel = new StudentFormViewModel
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
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Form(StudentFormViewModel model, IFormFile? document)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var form = await _context.StudentForms
            .FirstOrDefaultAsync(f => f.UserId == user.Id);

        if (form?.IsSubmitted == true)
        {
            return RedirectToAction(nameof(Index));
        }

        if (document != null)
        {
            using var ms = new MemoryStream();
            await document.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            model.UploadedDocumentBase64 = Convert.ToBase64String(fileBytes);
        }

        if (form == null)
        {
            form = new StudentForm
            {
                UserId = user.Id,
                MatricNumber = model.MatricNumber,
                Department = model.Department,
                Level = model.Level,
                UploadedDocument = model.UploadedDocumentBase64,
                IsSubmitted = model.IsSubmitted,
                LastUpdated = DateTime.UtcNow,
                SubmittedAt = model.IsSubmitted ? DateTime.UtcNow : null,
                DateOfBirth = model.DateOfBirth,
                JambRegNumber = model.JambRegNumber,
                JambScore = model.JambScore,
                LocalGov = model.LocalGov,
                MaritalStatus = model.MaritalStatus,
                ModeOfEntry = model.ModeOfEntry,
                Nationality = model.Nationality,
                NextOfKin = model.NextOfKin,
                NextOfKinPhoneNumber = model.NextOfKinPhoneNumber,
                PostUtmeScore = model.PostUtmeScore,
                State = model.State,
                WAECRegNumber = model.WAECRegNumber,
            };
            _context.StudentForms.Add(form);
        }
        else
        {
            form.MatricNumber = model.MatricNumber;
            form.Department = model.Department;
            form.Level = model.Level;
            if (model.UploadedDocumentBase64 != null)
            {
                form.UploadedDocument = model.UploadedDocumentBase64;
            }
            form.IsSubmitted = model.IsSubmitted;
            form.LastUpdated = DateTime.UtcNow;
            form.SubmittedAt = model.IsSubmitted ? DateTime.UtcNow : null;
            form.DateOfBirth = model.DateOfBirth;
            form.JambRegNumber = model.JambRegNumber;
            form.JambScore = model.JambScore;
            form.LocalGov = model.LocalGov;
            form.MaritalStatus = model.MaritalStatus;
            form.ModeOfEntry = model.ModeOfEntry;
            form.Nationality = model.Nationality;
            form.NextOfKin = model.NextOfKin;
            form.NextOfKinPhoneNumber = model.NextOfKinPhoneNumber;
            form.PostUtmeScore = model.PostUtmeScore;
            form.State = model.State;
            form.WAECRegNumber = model.WAECRegNumber;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}