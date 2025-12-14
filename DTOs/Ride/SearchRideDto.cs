namespace Pikmi.API.DTOs.Ride
{
    public class SearchRideDto
    {
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public DateTime? StartTime { get; set; }
    }
}
