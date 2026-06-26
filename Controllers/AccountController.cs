using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Services;
using Student_Portal.ViewModels;

namespace Student_Portal.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
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
            var result = await _accountService.RegisterStudentAsync(model);
            if (result.Succeeded)
            {
                TempData["Success"] = "Registration successful. Please wait for admin approval.";
                return RedirectToAction("RegisterConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
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
            var result = await _accountService.LoginAdminAsync(model);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Admin");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
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
            var result = await _accountService.LoginStudentAsync(model);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Student");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
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
        await _accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}
