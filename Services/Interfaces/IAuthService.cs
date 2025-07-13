using Microsoft.AspNetCore.Identity;
using Pikmi.API.DTOs.Identity;

namespace Pikmi.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string userId);
        Task<bool> GenerateEmailConfirmationTokenAsync(string email);
        Task<bool> ConfirmEmailAsync(string userId, string token);  
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
