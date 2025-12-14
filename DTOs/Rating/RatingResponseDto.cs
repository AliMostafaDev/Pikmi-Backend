namespace Pikmi.API.DTOs.Rating
{
    public class RatingResponseDto
    {
        public int RatingId { get; set; }
        public int RideId { get; set; }
        public string RaterId { get; set; }
        public string RaterName { get; set; }
        public string RaterProfileImage { get; set; }
        public string RatedUserId { get; set; }
        public string RatedUserName { get; set; }
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
