using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Ride
{
    public class UpdateRideDto
    {
        [MaxLength(500)]
        public string? FromLocation { get; set; }

        [MaxLength(500)]
        public string? ToLocation { get; set; }

        public DateTime? StartTime { get; set; }

        [Range(1, 10)]
        public int? SeatsAvailable { get; set; }

        [Range(0.01, 1000)]
        public decimal? CostInCoins { get; set; }
    }
}
