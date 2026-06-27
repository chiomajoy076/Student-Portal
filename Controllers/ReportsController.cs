using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Portal.Models;
using Student_Portal.Services;

namespace Student_Portal.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> RegistrationReport(string? department)
    {
        var report = await _reportService.GetRegistrationReportAsync(department);
        return View(report);
    }

    public async Task<IActionResult> ExportRegistrationPdf(string? department)
    {
        var report = await _reportService.GetRegistrationReportAsync(department);
        var pdf = _reportService.ExportRegistrationReportToPdf(report);
        return File(pdf, "application/pdf", "RegistrationReport.pdf");
    }

    public async Task<IActionResult> ExportRegistrationExcel(string? department)
    {
        var report = await _reportService.GetRegistrationReportAsync(department);
        var excel = _reportService.ExportRegistrationReportToExcel(report);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RegistrationReport.xlsx");
    }

    public async Task<IActionResult> ResultStatistics(string? session, Semester? semester, string? department)
    {
        var report = await _reportService.GetResultStatisticsAsync(session, semester, department);
        return View(report);
    }

    public async Task<IActionResult> ExportResultStatsPdf(string? session, Semester? semester, string? department)
    {
        var report = await _reportService.GetResultStatisticsAsync(session, semester, department);
        var pdf = _reportService.ExportResultStatisticsToPdf(report);
        return File(pdf, "application/pdf", "ResultStatistics.pdf");
    }

    public async Task<IActionResult> ExportResultStatsExcel(string? session, Semester? semester, string? department)
    {
        var report = await _reportService.GetResultStatisticsAsync(session, semester, department);
        var excel = _reportService.ExportResultStatisticsToExcel(report);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ResultStatistics.xlsx");
    }
}
