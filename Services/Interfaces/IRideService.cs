using Pikmi.API.Entities;

namespace Pikmi.API.Services.Interfaces
{
    public interface IRideService
    {
        Task<Ride?> GetRideByIdAsync(int rideId);
        Task<IEnumerable<Ride>> GetAllRidesAsync();
        Task<IEnumerable<Ride>> GetActiveRidesAsync();
        Task<IEnumerable<Ride>> GetDriverRidesAsync(string driverId);
        Task<IEnumerable<Ride>> SearchRidesAsync(string fromLocation, string toLocation, DateTime? startTime);
        Task<Ride> CreateRideAsync(Ride ride);
        Task<Ride> UpdateRideAsync(Ride ride);
        Task<bool> DeleteRideAsync(int rideId);
        Task<bool> CancelRideAsync(int rideId);
        Task<bool> CompleteRideAsync(int rideId);
    }
}
