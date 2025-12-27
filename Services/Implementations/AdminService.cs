using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pikmi.API.DTOs.Admin;
using Pikmi.API.Entities;
using Pikmi.API.Services.Interfaces;

namespace Pikmi.API.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AdminService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserWithRolesDto>> GetAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserWithRolesDto>(user);
                userDto.Roles = roles;
                usersWithRoles.Add(userDto);
            }

            return usersWithRoles;
        }

        public async Task<UserWithRolesDto?> GetUserWithRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserWithRolesDto>(user);
            userDto.Roles = roles;

            return userDto;
        }

        public async Task<IdentityResult> CreateAdminAsync(CreateAdminDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email already registered"
                });
            }

            var admin = _mapper.Map<ApplicationUser>(dto);

            var result = await _userManager.CreateAsync(admin, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
            }

            return result;
        }

        public async Task<IdentityResult> PromoteToAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "AlreadyAdmin",
                    Description = "User is already an admin"
                });
            }

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CannotModifySuperAdmin",
                    Description = "Cannot modify SuperAdmin role"
                });
            }

            return await _userManager.AddToRoleAsync(user, "Admin");
        }

        public async Task<IdentityResult> DemoteToUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CannotDemoteSuperAdmin",
                    Description = "Cannot demote SuperAdmin"
                });
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "NotAdmin",
                    Description = "User is not an admin"
                });
            }

            return await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        public async Task<IdentityResult> VerifyUserDocumentAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }

            user.IsDocumentVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> AddBalanceToUserAsync(string userId, decimal amount)
        {
            if (amount <= 0)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidAmount",
                    Description = "Amount must be greater than 0"
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }

            user.Balance += amount;
            user.UpdatedAt = DateTime.UtcNow;

            return await _userManager.UpdateAsync(user);
        }
    }
}
