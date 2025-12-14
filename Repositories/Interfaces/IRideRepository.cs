using Pikmi.API.Entities;

namespace Pikmi.API.Repositories.Interfaces
{
    public interface IRideRepository
    {
        Task<Ride?> GetByIdAsync(int rideId);
        Task<IEnumerable<Ride>> GetAllAsync();
        Task<IEnumerable<Ride>> GetActiveRidesAsync();
        Task<IEnumerable<Ride>> GetRidesByDriverAsync(string driverId);
        Task<IEnumerable<Ride>> SearchRidesAsync(string fromLocation, string toLocation, DateTime? startTime);
        Task<Ride> CreateAsync(Ride ride);
        Task<Ride> UpdateAsync(Ride ride);
        Task<bool> DeleteAsync(int rideId);
        Task<bool> UpdateRideStatusAsync(int rideId, string status);
    }
}
