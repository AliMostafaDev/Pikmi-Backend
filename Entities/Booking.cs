using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int RideId { get; set; }

        [Required]
        public string PassengerId { get; set; }

        [Required]
        [Range(1, 10)]
        public int SeatsBooked { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CoinsUsed { get; set; }

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(RideId))]
        public virtual Ride Ride { get; set; }

        [ForeignKey(nameof(PassengerId))]
        public virtual ApplicationUser Passenger { get; set; }
    }
}
