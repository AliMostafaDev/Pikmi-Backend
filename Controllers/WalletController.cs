using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pikmi.API.DTOs.Balance;
using Pikmi.API.Services.Interfaces;
using System.Security.Claims;

namespace Pikmi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;

        public WalletController(IWalletService walletService, IMapper mapper)
        {
            _walletService = walletService;
            _mapper = mapper;
        }

        [HttpGet("balance")]
        public async Task<ActionResult<BalanceResponseDto>> GetBalance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var balance = await _walletService.GetBalanceAsync(userId);

            var response = new BalanceResponseDto
            {
                UserId = userId,
                Balance = balance,
                UpdatedAt = DateTime.UtcNow
            };

            return Ok(response);
        }

        [HttpPost("add-balance")]
        public async Task<ActionResult<BalanceResponseDto>> AddBalance([FromBody] AddBalanceDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _walletService.AddBalanceAsync(userId, dto.Amount);

            if (!result)
                return BadRequest(new { message = "Failed to add balance" });

            var balance = await _walletService.GetBalanceAsync(userId);

            var response = new BalanceResponseDto
            {
                UserId = userId,
                Balance = balance,
                UpdatedAt = DateTime.UtcNow
            };

            return Ok(response);
        }

        [HttpGet("user/{userId}/balance")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<BalanceResponseDto>> GetUserBalance(string userId)
        {
            var balance = await _walletService.GetBalanceAsync(userId);

            var response = new BalanceResponseDto
            {
                UserId = userId,
                Balance = balance,
                UpdatedAt = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}
