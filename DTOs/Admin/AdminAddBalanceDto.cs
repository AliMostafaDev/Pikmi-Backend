using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Admin
{
    public class AdminAddBalanceDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Amount { get; set; }
    }
}
