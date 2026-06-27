using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Models;

namespace Student_Portal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (User.IsInRole("Lecturer"))
            {
                return RedirectToAction("Roster", "Results");
            }

            if (User.IsInRole("ExamOfficer"))
            {
                return RedirectToAction("UploadResult", "Results");
            }

            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Student");
            }
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}