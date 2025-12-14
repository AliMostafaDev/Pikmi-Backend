namespace Pikmi.API.Services.Interfaces
{
    public interface IWalletService
    {
        Task<decimal> GetBalanceAsync(string userId);
        Task<bool> HasSufficientBalanceAsync(string userId, decimal amount);
        Task<bool> DeductBalanceAsync(string userId, decimal amount);
        Task<bool> AddBalanceAsync(string userId, decimal amount);
        Task<bool> TransferBalanceAsync(string fromUserId, string toUserId, decimal amount);

    }
}
