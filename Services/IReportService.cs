using Student_Portal.Models;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public interface IReportService
{
    Task<RegistrationReportViewModel> GetRegistrationReportAsync(string? department, int page = 1, int pageSize = 20);
    Task<ResultStatisticsViewModel> GetResultStatisticsAsync(string? session, Semester? semester, string? department, int page = 1, int pageSize = 20);
    byte[] ExportRegistrationReportToPdf(RegistrationReportViewModel report);
    byte[] ExportRegistrationReportToExcel(RegistrationReportViewModel report);
    byte[] ExportResultStatisticsToPdf(ResultStatisticsViewModel report);
    byte[] ExportResultStatisticsToExcel(ResultStatisticsViewModel report);
}
