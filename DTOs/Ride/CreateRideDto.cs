using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Ride
{
    public class CreateRideDto
    {
        [Required]
        [MaxLength(500)]
        public string FromLocation { get; set; }

        [Required]
        [MaxLength(500)]
        public string ToLocation { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [Range(1, 10)]
        public int SeatsAvailable { get; set; }

        [Required]
        [Range(0.01, 1000)]
        public decimal CostInCoins { get; set; }
    }
}
