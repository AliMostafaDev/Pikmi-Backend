using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.Entities
{
    public class Ride
    {
        [Key]
        public int RideId { get; set; }

        [Required]
        public string DriverId { get; set; }

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
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostInCoins { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active, Completed, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(DriverId))]
        public virtual ApplicationUser Driver { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
