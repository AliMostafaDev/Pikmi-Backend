using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pikmi.API.DTOs.Admin;
using Pikmi.API.DTOs.User;
using Pikmi.API.Services.Interfaces;

namespace Pikmi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        public AdminController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }

        [HttpGet("users")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IEnumerable<UserWithRolesDto>>> GetAllUsersWithRoles()
        {
            var users = await _adminService.GetAllUsersWithRolesAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<UserWithRolesDto>> GetUserWithRoles(string id)
        {
            var user = await _adminService.GetUserWithRolesAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "User created successfully" });
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return NoContent();
        }

        [HttpPost("admins")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            var result = await _adminService.CreateAdminAsync(dto);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Admin created successfully" });
        }

        [HttpPost("promote")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> PromoteToAdmin([FromBody] ManageRoleDto dto)
        {
            var result = await _adminService.PromoteToAdminAsync(dto.UserId);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "User promoted to Admin successfully" });
        }

        [HttpPost("demote")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DemoteToUser([FromBody] ManageRoleDto dto)
        {
            var result = await _adminService.DemoteToUserAsync(dto.UserId);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Admin demoted to User successfully" });
        }


        [HttpPost("verify-document")]
        public async Task<IActionResult> VerifyUserDocument([FromBody] VerifyDocumentDto dto)
        {
            var result = await _adminService.VerifyUserDocumentAsync(dto.UserId);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "User document verified successfully" });
        }


        [HttpPost("add-balance")]
        public async Task<IActionResult> AddBalanceToUser([FromBody] AdminAddBalanceDto dto)
        {
            var result = await _adminService.AddBalanceToUserAsync(dto.UserId, dto.Amount);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = $"Added {dto.Amount} coins successfully" });
        }
    }
}
