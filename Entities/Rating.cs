using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.Entities
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public int RideId { get; set; }

        [Required]
        public string RaterId { get; set; }

        [Required]
        public string RatedUserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(RideId))]
        public virtual Ride Ride { get; set; }

        [ForeignKey(nameof(RaterId))]
        public virtual ApplicationUser Rater { get; set; }

        [ForeignKey(nameof(RatedUserId))]
        public virtual ApplicationUser RatedUser { get; set; }
    }
}
