using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Pikmi.API.DTOs.Identity;
using Pikmi.API.Entities;
using Pikmi.API.Services.Interfaces;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Pikmi.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, IEmailService emailService, IMapper mapper)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var user = _mapper.Map<ApplicationUser>(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Confirm Your Email",
                    $"Your email confirmation token is:<br><br><b>{WebUtility.HtmlEncode(token)}</b><br><br>Use this token to confirm your email.",
                    isHtml: true);

            }
            return result;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password) || !user.EmailConfirmed)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(_config.GetValue<int>("JwtSettings:AccessTokenExpiryInMinutes")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var newRefreshToken = Guid.NewGuid().ToString(); 
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.GetValue<int>("JwtSettings:RefreshTokenExpiryInDays"));
            await _userManager.UpdateAsync(user);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = newRefreshToken,
                ExpireAt = token.ValidTo
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(_config.GetValue<int>("JwtSettings:AccessTokenExpiryInMinutes")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var newRefreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.GetValue<int>("JwtSettings:RefreshTokenExpiryInDays"));
            await _userManager.UpdateAsync(user);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = newRefreshToken,
                ExpireAt = token.ValidTo
            };
        
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return false;

            return await _emailService.SendEmailAsync(
                user.Email,
                "Email Confirmed Successfully",
                "Your email has been successfully confirmed. You can now log in to your account.",
                isHtml: true);
        }

        public async Task<bool> GenerateEmailConfirmationTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
                return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return await _emailService.SendEmailAsync(
                    user.Email,
                    "Confirm Your Email",
                    $"Your email confirmation token is:<br><br><b>{WebUtility.HtmlEncode(token)}</b><br><br>Use this token to confirm your email.",
                    isHtml: true);   
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return await _emailService.SendEmailAsync(
                email, 
                "Reset Your Password", 
                $"Here is you password reset token: <br><br>{WebUtility.HtmlEncode(token)}<br><br>Use it to reset your password via the application",
                isHtml: true
                );   
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (result.Succeeded)
            {
                await _emailService.SendEmailAsync(
                    dto.Email,
                    "Password Changed Successfully",
                    "Your password has been successfully changed",
                    isHtml: true
                    );
            }
            return result.Succeeded;
        }
        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;   

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
      
    }
}
