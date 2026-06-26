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
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<IdentityResult> UpdateAsync(ApplicationUser user);
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
    Task<bool> IsInRoleAsync(ApplicationUser user, string role);
}
