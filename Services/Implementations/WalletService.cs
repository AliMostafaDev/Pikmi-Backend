using Microsoft.AspNetCore.Identity;
using Pikmi.API.Entities;

namespace Pikmi.API.Services.Implementations
{
    public class WalletService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public WalletService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<decimal> GetBalanceAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Balance ?? 0;
        }

        public async Task<bool> HasSufficientBalanceAsync(string userId, decimal amount)
        {
            var balance = await GetBalanceAsync(userId);
            return balance >= amount;
        }

        public async Task<bool> DeductBalanceAsync(string userId, decimal amount)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Balance < amount)
                return false;

            user.Balance -= amount;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> AddBalanceAsync(string userId, decimal amount)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.Balance += amount;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> TransferBalanceAsync(string fromUserId, string toUserId, decimal amount)
        {
            if (!await HasSufficientBalanceAsync(fromUserId, amount))
                return false;

            await DeductBalanceAsync(fromUserId, amount);
            await AddBalanceAsync(toUserId, amount);
            return true;
        }
    }
}
