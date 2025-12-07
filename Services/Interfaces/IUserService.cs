using Microsoft.AspNetCore.Identity;
using Pikmi.API.DTOs.User;

namespace Pikmi.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<IdentityResult> CreateUserAsync(CreateUserDto dto);
        Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto dto);
        Task<IdentityResult> DeleteUserAsync(string id);
    }
}
