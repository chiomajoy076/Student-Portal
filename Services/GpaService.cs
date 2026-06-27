using Student_Portal.Models;
using Student_Portal.Repositories;

namespace Student_Portal.Services;

public class GpaService : IGpaService
{
    private readonly IResultRepository _resultRepository;
    private readonly IGpaRecordRepository _gpaRecordRepository;

    public GpaService(IResultRepository resultRepository, IGpaRecordRepository gpaRecordRepository)
    {
        _resultRepository = resultRepository;
        _gpaRecordRepository = gpaRecordRepository;
    }

    public async Task RecalculateForStudentAsync(string userId)
    {
        var results = await _resultRepository.GetByUserIdAsync(userId);

        // AE-FUNAI sessions are formatted "YYYY/YYYY" so lexical ordering matches chronological order.
        var semesterGroups = results
            .GroupBy(r => (r.Course.Session, r.Course.Semester))
            .OrderBy(g => g.Key.Session)
            .ThenBy(g => g.Key.Semester);

        decimal cumulativePoints = 0;
        decimal cumulativeUnits = 0;

        foreach (var group in semesterGroups)
        {
            decimal semesterPoints = 0;
            decimal semesterUnits = 0;

            foreach (var result in group)
            {
                semesterPoints += (int)result.Grade * result.Course.CreditUnit;
                semesterUnits += result.Course.CreditUnit;
            }

            cumulativePoints += semesterPoints;
            cumulativeUnits += semesterUnits;

            var gpa = semesterUnits == 0 ? 0 : Math.Round(semesterPoints / semesterUnits, 2);
            var cgpa = cumulativeUnits == 0 ? 0 : Math.Round(cumulativePoints / cumulativeUnits, 2);

            var (session, semester) = group.Key;
            var record = await _gpaRecordRepository.GetByUserSessionSemesterAsync(userId, session, semester);

            if (record == null)
            {
                await _gpaRecordRepository.AddAsync(new GpaRecord
                {
                    UserId = userId,
                    Session = session,
                    Semester = semester,
                    GPA = gpa,
                    CGPA = cgpa,
                    ComputedAt = DateTime.UtcNow
                });
            }
            else
            {
                record.GPA = gpa;
                record.CGPA = cgpa;
                record.ComputedAt = DateTime.UtcNow;
            }
        }

        await _gpaRecordRepository.SaveChangesAsync();
    }
}
