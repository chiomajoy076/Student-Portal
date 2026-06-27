namespace Student_Portal.Models;

public static class AcademicSessions
{
    public static List<string> GetSelectableSessions()
    {
        var currentYear = DateTime.UtcNow.Year;
        var sessions = new List<string>();

        for (var year = currentYear - 3; year <= currentYear + 1; year++)
        {
            sessions.Add($"{year}/{year + 1}");
        }

        return sessions;
    }
}
