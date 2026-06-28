using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByIdAsync(string id);
    Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);
    Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role);
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<IdentityResult> UpdateAsync(ApplicationUser user);
    Task<IdentityResult> DeleteAsync(ApplicationUser user);
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
    Task<bool> IsInRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);
    Task<bool> IsLockedOutAsync(ApplicationUser user);
    Task<IdentityResult> SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd);
    Task<IdentityResult> SetLockoutEnabledAsync(ApplicationUser user, bool enabled);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
}
