namespace Student_Portal.Services;

public interface IRegistrationSlipService
{
    Task<byte[]?> GenerateAsync(string userId);
}
