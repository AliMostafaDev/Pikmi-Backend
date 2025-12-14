namespace Pikmi.API.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int RideId { get; set; }
        public string PassengerId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerProfileImage { get; set; }
        public int SeatsBooked { get; set; }
        public string Status { get; set; }
        public decimal CoinsUsed { get; set; }
        public DateTime BookedAt { get; set; }

        // Ride Details
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime StartTime { get; set; }
        public string DriverName { get; set; }
        public string DriverId { get; set; }
    }
}
