using System.Globalization;
using CsvHelper;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.ViewModels;

namespace Student_Portal.Services;

public class ResultService : IResultService
{
    private readonly IResultRepository _resultRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IGpaService _gpaService;
    private readonly IGpaRecordRepository _gpaRecordRepository;
    private readonly IAuditService _auditService;

    public ResultService(IResultRepository resultRepository, ICourseRepository courseRepository,
        IStudentRepository studentRepository, IGpaService gpaService, IGpaRecordRepository gpaRecordRepository,
        IAuditService auditService)
    {
        _resultRepository = resultRepository;
        _courseRepository = courseRepository;
        _studentRepository = studentRepository;
        _gpaService = gpaService;
        _gpaRecordRepository = gpaRecordRepository;
        _auditService = auditService;
    }

    public async Task<ServiceResult> UploadSingleResultAsync(ResultUploadViewModel model)
    {
        var student = await _studentRepository.GetByMatricNumberAsync(model.MatricNumber);
        if (student == null)
        {
            return ServiceResult.Fail($"No student found with matric number '{model.MatricNumber}'.");
        }

        var course = await _courseRepository.GetByIdAsync(model.CourseId);
        if (course == null)
        {
            return ServiceResult.Fail("Selected course was not found.");
        }

        if (await _resultRepository.ExistsAsync(student.UserId, course.Id))
        {
            return ServiceResult.Fail($"A result for '{model.MatricNumber}' in '{course.CourseCode}' already exists.");
        }

        var result = new Result
        {
            UserId = student.UserId,
            CourseId = course.Id,
            Score = model.Score,
            Grade = GradeCalculator.FromScore(model.Score),
            UploadedAt = DateTime.UtcNow
        };

        await _resultRepository.AddAsync(result);
        await _resultRepository.SaveChangesAsync();

        await _gpaService.RecalculateForStudentAsync(student.UserId);
        await _auditService.LogAsync(student.UserId, $"Result uploaded for course '{course.CourseCode}' (score: {model.Score})");

        return ServiceResult.Success();
    }

    public async Task<ResultImportSummaryViewModel> ImportFromCsvAsync(IFormFile file)
    {
        var summary = new ResultImportSummaryViewModel();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        List<ResultCsvRow> rows;
        try
        {
            rows = csv.GetRecords<ResultCsvRow>().ToList();
        }
        catch (Exception ex)
        {
            summary.RowErrors.Add($"Could not parse CSV file: {ex.Message}");
            return summary;
        }

        summary.TotalRows = rows.Count;
        var seenInBatch = new HashSet<(string UserId, int CourseId)>();
        var affectedUserIds = new HashSet<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowNumber = i + 2; // account for header row

            if (!Enum.TryParse<Semester>(row.Semester, true, out var semester))
            {
                summary.RowErrors.Add($"Row {rowNumber}: invalid semester '{row.Semester}'.");
                continue;
            }

            if (row.Score < 0 || row.Score > 100)
            {
                summary.RowErrors.Add($"Row {rowNumber}: score {row.Score} is out of range (0-100).");
                continue;
            }

            var student = await _studentRepository.GetByMatricNumberAsync(row.MatricNumber);
            if (student == null)
            {
                summary.RowErrors.Add($"Row {rowNumber}: no student found with matric number '{row.MatricNumber}'.");
                continue;
            }

            var course = await _courseRepository.FindAsync(row.CourseCode, row.Session, semester);
            if (course == null)
            {
                summary.RowErrors.Add($"Row {rowNumber}: course '{row.CourseCode}' not found for {row.Session} {row.Semester} semester.");
                continue;
            }

            if (await _resultRepository.ExistsAsync(student.UserId, course.Id) ||
                !seenInBatch.Add((student.UserId, course.Id)))
            {
                summary.RowErrors.Add($"Row {rowNumber}: result for '{row.MatricNumber}' in '{row.CourseCode}' already exists.");
                continue;
            }

            await _resultRepository.AddAsync(new Result
            {
                UserId = student.UserId,
                CourseId = course.Id,
                Score = row.Score,
                Grade = GradeCalculator.FromScore(row.Score),
                UploadedAt = DateTime.UtcNow
            });

            affectedUserIds.Add(student.UserId);
            summary.SuccessCount++;
        }

        await _resultRepository.SaveChangesAsync();

        foreach (var userId in affectedUserIds)
        {
            await _gpaService.RecalculateForStudentAsync(userId);
        }

        await _auditService.LogAsync(null,
            $"Bulk result import processed: {summary.SuccessCount}/{summary.TotalRows} succeeded");

        return summary;
    }

    public async Task<List<AcademicPeriod>> GetAvailablePeriodsAsync(string userId)
    {
        var results = await _resultRepository.GetByUserIdAsync(userId);

        return results
            .Select(r => new AcademicPeriod { Session = r.Course.Session, Semester = r.Course.Semester })
            .Distinct()
            .OrderByDescending(p => p.Session)
            .ThenByDescending(p => p.Semester)
            .ToList();
    }

    public async Task<ResultCheckViewModel?> GetStudentResultAsync(string userId, string session, Semester semester)
    {
        var results = await _resultRepository.GetByUserIdAsync(userId);
        var periodResults = results
            .Where(r => r.Course.Session == session && r.Course.Semester == semester)
            .ToList();

        if (periodResults.Count == 0)
        {
            return null;
        }

        var gpaRecord = await _gpaRecordRepository.GetByUserSessionSemesterAsync(userId, session, semester);

        return new ResultCheckViewModel
        {
            Session = session,
            Semester = semester,
            GPA = gpaRecord?.GPA ?? 0,
            CGPA = gpaRecord?.CGPA ?? 0,
            Courses = periodResults.Select(r => new CourseResultRow
            {
                CourseCode = r.Course.CourseCode,
                CourseTitle = r.Course.CourseTitle,
                CreditUnit = r.Course.CreditUnit,
                Score = r.Score,
                Grade = r.Grade
            }).ToList()
        };
    }
}
