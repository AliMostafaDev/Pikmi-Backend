namespace Pikmi.API.DTOs.Ride
{
    public class RideResponseDto
    {
        public int RideId { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string DriverProfileImage { get; set; }
        public double DriverRating { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime StartTime { get; set; }
        public int SeatsAvailable { get; set; }
        public decimal CostInCoins { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalBookings { get; set; }
    }
}
