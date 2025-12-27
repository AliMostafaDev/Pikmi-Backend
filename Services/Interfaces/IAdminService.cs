using Microsoft.AspNetCore.Identity;
using Pikmi.API.DTOs.Admin;

namespace Pikmi.API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserWithRolesDto>> GetAllUsersWithRolesAsync();
        Task<UserWithRolesDto?> GetUserWithRolesAsync(string userId);

        Task<IdentityResult> CreateAdminAsync(CreateAdminDto dto);

        Task<IdentityResult> PromoteToAdminAsync(string userId);
        Task<IdentityResult> DemoteToUserAsync(string userId);

        Task<IdentityResult> VerifyUserDocumentAsync(string userId);

        Task<IdentityResult> AddBalanceToUserAsync(string userId, decimal amount);
    }
}
