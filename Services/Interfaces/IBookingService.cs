using Pikmi.API.Entities;

namespace Pikmi.API.Services.Interfaces
{
    public interface IBookingService 
    {
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<Booking>> GetPassengerBookingsAsync(string passengerId);
        Task<IEnumerable<Booking>> GetRideBookingsAsync(int rideId);
        Task<Booking> CreateBookingAsync(int rideId, string passengerId, int seatsBooked);
        Task<bool> ConfirmBookingAsync(int bookingId);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<bool> CompleteBookingAsync(int bookingId);
    }
}
