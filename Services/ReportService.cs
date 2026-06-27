using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;
using Grade = Student_Portal.Models.Grade;
using Semester = Student_Portal.Models.Semester;
using PdfDocument = QuestPDF.Fluent.Document;

namespace Student_Portal.Services;

public class ReportService : IReportService
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IResultRepository _resultRepository;

    public ReportService(IUserRepository userRepository, IStudentRepository studentRepository,
        IResultRepository resultRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _resultRepository = resultRepository;
    }

    public async Task<RegistrationReportViewModel> GetRegistrationReportAsync(string? department)
    {
        var students = await _userRepository.GetUsersInRoleAsync("Student");
        var forms = await _studentRepository.GetAllAsync();

        var rows = forms
            .Select(f =>
            {
                var student = students.FirstOrDefault(s => s.Id == f.UserId);
                return new RegistrationRow
                {
                    MatricNumber = f.MatricNumber,
                    FullName = student == null ? "(unknown)" : $"{student.FirstName} {student.LastName}",
                    Department = f.Department,
                    Level = f.Level,
                    RegisteredAt = f.SubmittedAt,
                    IsSubmitted = f.IsSubmitted
                };
            })
            .Where(r => string.IsNullOrWhiteSpace(department) ||
                        string.Equals(r.Department, department, StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.Department)
            .ThenBy(r => r.FullName)
            .ToList();

        return new RegistrationReportViewModel
        {
            DepartmentFilter = department,
            Rows = rows,
            TotalCount = rows.Count,
            CountsByDepartment = rows
                .GroupBy(r => r.Department ?? "(none)")
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public async Task<ResultStatisticsViewModel> GetResultStatisticsAsync(string? session, Semester? semester, string? department)
    {
        var results = await _resultRepository.GetAllAsync();

        var filtered = results.Where(r =>
            (string.IsNullOrWhiteSpace(session) || r.Course.Session == session) &&
            (semester == null || r.Course.Semester == semester) &&
            (string.IsNullOrWhiteSpace(department) || string.Equals(r.Course.Department, department, StringComparison.OrdinalIgnoreCase)));

        var courses = filtered
            .GroupBy(r => r.Course)
            .Select(g => new CourseStatistic
            {
                CourseCode = g.Key.CourseCode,
                CourseTitle = g.Key.CourseTitle,
                Department = g.Key.Department,
                Session = g.Key.Session,
                Semester = g.Key.Semester.ToString(),
                TotalStudents = g.Count(),
                AverageScore = Math.Round((decimal)g.Average(r => r.Score), 2),
                PassCount = g.Count(r => r.Grade != Grade.F),
                FailCount = g.Count(r => r.Grade == Grade.F)
            })
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToList();

        return new ResultStatisticsViewModel
        {
            Session = session,
            Semester = semester?.ToString(),
            Department = department,
            Courses = courses
        };
    }

    public byte[] ExportRegistrationReportToPdf(RegistrationReportViewModel report)
    {
        var document = PdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Text("AE-FUNAI Registration Report").Bold().FontSize(16);

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                    });

                    foreach (var header in new[] { "Matric No.", "Name", "Department", "Level", "Registered", "Submitted" })
                    {
                        table.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text(header).Bold();
                    }

                    foreach (var row in report.Rows)
                    {
                        table.Cell().Padding(3).Text(row.MatricNumber ?? "");
                        table.Cell().Padding(3).Text(row.FullName);
                        table.Cell().Padding(3).Text(row.Department ?? "");
                        table.Cell().Padding(3).Text(row.Level ?? "");
                        table.Cell().Padding(3).Text(row.RegisteredAt?.ToString("yyyy-MM-dd") ?? "-");
                        table.Cell().Padding(3).Text(row.IsSubmitted ? "Yes" : "No");
                    }
                });

                page.Footer().AlignRight().Text($"Total: {report.TotalCount} student(s)").FontSize(10);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] ExportRegistrationReportToExcel(RegistrationReportViewModel report)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Registration Report");

        string[] headers = { "Matric No.", "Name", "Department", "Level", "Registered", "Submitted" };
        for (var i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
            sheet.Cell(1, i + 1).Style.Font.Bold = true;
        }

        var rowIndex = 2;
        foreach (var row in report.Rows)
        {
            sheet.Cell(rowIndex, 1).Value = row.MatricNumber;
            sheet.Cell(rowIndex, 2).Value = row.FullName;
            sheet.Cell(rowIndex, 3).Value = row.Department;
            sheet.Cell(rowIndex, 4).Value = row.Level;
            sheet.Cell(rowIndex, 5).Value = row.RegisteredAt?.ToString("yyyy-MM-dd") ?? "-";
            sheet.Cell(rowIndex, 6).Value = row.IsSubmitted ? "Yes" : "No";
            rowIndex++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportResultStatisticsToPdf(ResultStatisticsViewModel report)
    {
        var document = PdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Text("AE-FUNAI Result Statistics").Bold().FontSize(16);

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    foreach (var header in new[] { "Code", "Title", "Department", "Students", "Avg Score", "Pass", "Fail" })
                    {
                        table.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text(header).Bold();
                    }

                    foreach (var course in report.Courses)
                    {
                        table.Cell().Padding(3).Text(course.CourseCode);
                        table.Cell().Padding(3).Text(course.CourseTitle);
                        table.Cell().Padding(3).Text(course.Department ?? "");
                        table.Cell().Padding(3).Text(course.TotalStudents.ToString());
                        table.Cell().Padding(3).Text(course.AverageScore.ToString("0.00"));
                        table.Cell().Padding(3).Text(course.PassCount.ToString());
                        table.Cell().Padding(3).Text(course.FailCount.ToString());
                    }
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] ExportResultStatisticsToExcel(ResultStatisticsViewModel report)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Result Statistics");

        string[] headers = { "Code", "Title", "Department", "Session", "Semester", "Students", "Avg Score", "Pass", "Fail" };
        for (var i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
            sheet.Cell(1, i + 1).Style.Font.Bold = true;
        }

        var rowIndex = 2;
        foreach (var course in report.Courses)
        {
            sheet.Cell(rowIndex, 1).Value = course.CourseCode;
            sheet.Cell(rowIndex, 2).Value = course.CourseTitle;
            sheet.Cell(rowIndex, 3).Value = course.Department;
            sheet.Cell(rowIndex, 4).Value = course.Session;
            sheet.Cell(rowIndex, 5).Value = course.Semester;
            sheet.Cell(rowIndex, 6).Value = course.TotalStudents;
            sheet.Cell(rowIndex, 7).Value = course.AverageScore;
            sheet.Cell(rowIndex, 8).Value = course.PassCount;
            sheet.Cell(rowIndex, 9).Value = course.FailCount;
            rowIndex++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
