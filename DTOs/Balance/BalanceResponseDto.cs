namespace Pikmi.API.DTOs.Balance
{
    public class BalanceResponseDto
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
