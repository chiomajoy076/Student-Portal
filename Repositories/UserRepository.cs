using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Student_Portal.Models;

namespace Student_Portal.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<ApplicationUser?> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);

    public Task<ApplicationUser?> FindByIdAsync(string id) => _userManager.FindByIdAsync(id);

    public Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal) => _userManager.GetUserAsync(principal);

    public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role) => _userManager.GetUsersInRoleAsync(role);

    public Task<List<ApplicationUser>> GetAllUsersAsync() => Task.FromResult(_userManager.Users.ToList());

    public Task<IdentityResult> CreateAsync(ApplicationUser user, string password) =>
        _userManager.CreateAsync(user, password);

    public Task<IdentityResult> UpdateAsync(ApplicationUser user) => _userManager.UpdateAsync(user);

    public Task<IdentityResult> DeleteAsync(ApplicationUser user) => _userManager.DeleteAsync(user);

    public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) =>
        _userManager.AddToRoleAsync(user, role);

    public Task<bool> IsInRoleAsync(ApplicationUser user, string role) => _userManager.IsInRoleAsync(user, role);

    public Task<IList<string>> GetRolesAsync(ApplicationUser user) => _userManager.GetRolesAsync(user);

    public Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles) =>
        _userManager.RemoveFromRolesAsync(user, roles);

    public Task<bool> IsLockedOutAsync(ApplicationUser user) => _userManager.IsLockedOutAsync(user);

    public Task<IdentityResult> SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd) =>
        _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

    public Task<IdentityResult> SetLockoutEnabledAsync(ApplicationUser user, bool enabled) =>
        _userManager.SetLockoutEnabledAsync(user, enabled);
}
