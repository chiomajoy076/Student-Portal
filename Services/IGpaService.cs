namespace Student_Portal.Services;

public interface IGpaService
{
    Task RecalculateForStudentAsync(string userId);
}
