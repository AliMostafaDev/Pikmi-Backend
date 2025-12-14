using System.ComponentModel.DataAnnotations;

namespace Pikmi.API.DTOs.Rating
{
    public class CreateRatingDto
    {
        [Required]
        public int RideId { get; set; }

        [Required]
        public string RatedUserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
