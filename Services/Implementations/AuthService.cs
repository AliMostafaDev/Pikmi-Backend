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
            if (!result.Succeeded)
                return result;

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebUtility.UrlEncode(token);
            var encodedUserId = WebUtility.UrlEncode(user.Id);

            var confirmUrl =
                $"http://localhost:3000/confirm-email?userId={encodedUserId}&token={encodedToken}";

            var body = $@"
                 <p>Welcome to our application 👋</p>
                 <p>Please confirm your email address by clicking the link below:</p>
                 <p>
                     <a href=""{confirmUrl}"">Confirm Email</a>
                 </p>
                 <p>If you did not create this account, you can ignore this email.</p>
             ";

            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm Your Email",
                body,
                isHtml: true
            );

            return result;
            //var user = _mapper.Map<ApplicationUser>(dto);

            //var result = await _userManager.CreateAsync(user, dto.Password);

            //if (result.Succeeded)
            //{
            //    await _userManager.AddToRoleAsync(user, "User");

            //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //    await _emailService.SendEmailAsync(
            //        user.Email,
            //        "Confirm Your Email",
            //        $"Your email confirmation token is:<br><br><b>{WebUtility.HtmlEncode(token)}</b><br><br>Use this token to confirm your email.<br/>" +
            //        $"http://localhost:/3000/reset-password?token={WebUtility.HtmlEncode(token)}@email={user.Email}",
            //        isHtml: true);

            //}
            //return result;
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

            var encodedToken = WebUtility.UrlEncode(token);
            var encodedUserId = WebUtility.UrlEncode(user.Id);

            var confirmUrl =
                $"http://localhost:3000/confirm-email?userId={encodedUserId}&token={encodedToken}";

            var body = $@"
                <p>Thank you for registering.</p>
                <p>Please confirm your email address by clicking the link below:</p>
                <p>
                    <a href=""{confirmUrl}"">Confirm Email</a>
                </p>
                <p>If you did not create this account, you can safely ignore this email.</p>
            ";

            return await _emailService.SendEmailAsync(
                user.Email,
                "Confirm Your Email",
                body,
                isHtml: true
            );
            //var user = await _userManager.FindByEmailAsync(email);
            //if (user == null || user.EmailConfirmed)
            //    return false;

            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //return await _emailService.SendEmailAsync(
            //        user.Email,
            //        "Confirm Your Email",
            //        $"Your email confirmation token is:<br><br><b>{WebUtility.HtmlEncode(token)}</b><br><br>Use this token to confirm your email.",
            //        isHtml: true);   
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(user.Email);

            var resetUrl =
                $"http://localhost:3000/reset-password?token={encodedToken}&email={encodedEmail}";

            var body = $@"
                 <p>You requested a password reset.</p>
                 <p>Click the link below to reset your password:</p>
                 <p>
                     <a href=""{resetUrl}"">Reset Password</a>
                 </p>
                 <p>This link will expire soon.</p>
             ";

            return await _emailService.SendEmailAsync(
                email,
                "Reset Your Password",
                body,
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
