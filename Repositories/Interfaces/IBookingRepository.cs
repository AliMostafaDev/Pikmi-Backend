using Pikmi.API.Entities;

namespace Pikmi.API.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetBookingsByPassengerAsync(string passengerId);
        Task<IEnumerable<Booking>> GetBookingsByRideAsync(int rideId);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task<bool> DeleteAsync(int bookingId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
        Task<bool> BookingExistsAsync(int rideId, string passengerId);
    }
}
