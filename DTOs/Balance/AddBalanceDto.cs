using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Balance
{
    public class AddBalanceDto
    {
        [Required]
        [Range(1, 10000)]
        public decimal Amount { get; set; }
    }
}
