using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Required]
        public int RideId { get; set; }

        [Required]
        [Range(1, 10)]
        public int SeatsBooked { get; set; }
    }
}
