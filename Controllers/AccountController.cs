using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Models;
using Student_Portal.Services;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult StudentRegister()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> StudentRegister(StudentRegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "Email Address already exist.");
                return View(model);
            }

            user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                IsActive = false,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");

                //Mail is not working

                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var confirmationLink = Url.Action("ConfirmEmail", "Account",
                //    new { userId = user.Id, token = token }, Request.Scheme);

                //await _emailService.SendEmailAsync(model.Email, "Confirm your email",
                //    $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>");

                return RedirectToAction("RegisterConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult AdminLogin()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> AdminLogin(LoginViewModel model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Check if user is in Admin or SuperAdmin role
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");

            if (!isAdmin && !isSuperAdmin)
            {
                ModelState.AddModelError(string.Empty, "You are not authorized to access admin area.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email,
                model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }


    [HttpGet]
    public IActionResult StudentLogin()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> StudentLogin(LoginViewModel model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt One.");
                return View(model);
            }

            // Check if user is a Student
            var isStudent = await _userManager.IsInRoleAsync(user, "Student");
            if (!isStudent)
            {
                ModelState.AddModelError(string.Empty, "This login is for students only.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account is not activated yet. Please wait for admin approval.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email,
                model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Student");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}