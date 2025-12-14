using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pikmi.API.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        public string? ProfileImage { get; set; }

        [Range(0, 5)]
        public double AverageRating { get; set; }

        public bool IsDocumentVerified { get; set; } = false;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 100;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }


        public virtual ICollection<Ride> DriverRides { get; set; }
        public virtual ICollection<Booking> PassengerBookings { get; set; }
        public virtual ICollection<Rating> GivenRatings { get; set; }
        public virtual ICollection<Rating> ReceivedRatings { get; set; }
    }
}
