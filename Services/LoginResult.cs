namespace Student_Portal.Services;

public class LoginResult
{
    public bool Succeeded { get; private set; }
    public bool RequiresPasswordChange { get; private set; }
    public string? Email { get; private set; }
    public string? ResetToken { get; private set; }
    public List<string> Errors { get; private set; } = new();

    public static LoginResult Success() => new() { Succeeded = true };

    public static LoginResult Fail(string error) => new() { Succeeded = false, Errors = { error } };

    public static LoginResult RequirePasswordChange(string email, string token) =>
        new() { Succeeded = false, RequiresPasswordChange = true, Email = email, ResetToken = token };
}
