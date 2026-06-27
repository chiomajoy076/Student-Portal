namespace Student_Portal.Services;

public class ServiceResult
{
    public bool Succeeded { get; private set; }
    public List<string> Errors { get; private set; } = new();

    public static ServiceResult Success() => new() { Succeeded = true };

    public static ServiceResult Fail(string error) => new() { Succeeded = false, Errors = { error } };

    public static ServiceResult Fail(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors.ToList() };
}
