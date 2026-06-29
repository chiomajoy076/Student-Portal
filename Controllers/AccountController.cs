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

                // Home/Index redirects to the right dashboard based on the signed-in user's role.
                return RedirectToAction("Index", "Home");
            }

            if (result.RequiresPasswordChange)
            {
                TempData["Success"] = "Your account was just created by an administrator. Please set your own password before continuing.";
                return RedirectToAction("ResetPassword", new { email = result.Email, token = result.ResetToken, context = "admin", forced = true });
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

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        ViewBag.Context = "student";
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        ViewBag.Context = "student";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = await _accountService.ForgotPasswordAsync(model.Email);
        if (!request.Found || request.Blocked)
        {
            ModelState.AddModelError(string.Empty, request.Error!);
            return View(model);
        }

        return RedirectToAction("ResetPassword", new { email = request.Email, token = request.Token, context = "student" });
    }

    [HttpGet]
    public IActionResult AdminForgotPassword()
    {
        ViewBag.Context = "admin";
        return View("ForgotPassword", new ForgotPasswordViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> AdminForgotPassword(ForgotPasswordViewModel model)
    {
        ViewBag.Context = "admin";
        if (!ModelState.IsValid)
        {
            return View("ForgotPassword", model);
        }

        var request = await _accountService.ForgotPasswordAsync(model.Email);
        if (!request.Found || request.Blocked)
        {
            ModelState.AddModelError(string.Empty, request.Error!);
            return View("ForgotPassword", model);
        }

        return RedirectToAction("ResetPassword", new { email = request.Email, token = request.Token, context = "admin" });
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string token, string context = "student", bool forced = false)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction(context == "admin" ? "AdminForgotPassword" : "ForgotPassword");
        }

        ViewBag.Forced = forced;
        ViewBag.Context = context == "admin" ? "admin" : "student";
        return View(new ResetPasswordViewModel { Email = email, Token = token, Context = context == "admin" ? "admin" : "student" });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        ViewBag.Context = model.Context == "admin" ? "admin" : "student";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _accountService.ResetPasswordAsync(model);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        return RedirectToAction("ResetPasswordConfirmation", new { context = model.Context });
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation(string context = "student")
    {
        ViewBag.Context = context == "admin" ? "admin" : "student";
        return View();
    }
}
