using Student_Portal.Models;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IReportService
{
    Task<RegistrationReportViewModel> GetRegistrationReportAsync(string? department);
    Task<ResultStatisticsViewModel> GetResultStatisticsAsync(string? session, Semester? semester, string? department);
    byte[] ExportRegistrationReportToPdf(RegistrationReportViewModel report);
    byte[] ExportRegistrationReportToExcel(RegistrationReportViewModel report);
    byte[] ExportResultStatisticsToPdf(ResultStatisticsViewModel report);
    byte[] ExportResultStatisticsToExcel(ResultStatisticsViewModel report);
}
