namespace Student_Portal.Services;

public class PasswordResetRequest
{
    public bool Found { get; private set; }
    public bool Blocked { get; private set; }
    public string? Email { get; private set; }
    public string? Token { get; private set; }
    public string? Error { get; private set; }

    public static PasswordResetRequest NotFound() =>
        new() { Found = false, Error = "No account found with that email address." };

    public static PasswordResetRequest BlockedForSuperAdmin() =>
        new() { Found = true, Blocked = true, Error = "Password reset isn't available for this account. Contact the system administrator directly." };

    public static PasswordResetRequest Ready(string email, string token) =>
        new() { Found = true, Email = email, Token = token };
}
