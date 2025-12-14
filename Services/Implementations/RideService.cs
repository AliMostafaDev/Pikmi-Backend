using Pikmi.API.Entities;
using Pikmi.API.Repositories.Interfaces;

namespace Pikmi.API.Services.Implementations
{
    public class RideService
    {
        private readonly IRideRepository _rideRepository;

        public RideService(IRideRepository rideRepository)
        {
            _rideRepository = rideRepository;
        }

        public async Task<Ride?> GetRideByIdAsync(int rideId)
        {
            return await _rideRepository.GetByIdAsync(rideId);
        }

        public async Task<IEnumerable<Ride>> GetAllRidesAsync()
        {
            return await _rideRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Ride>> GetActiveRidesAsync()
        {
            return await _rideRepository.GetActiveRidesAsync();
        }

        public async Task<IEnumerable<Ride>> GetDriverRidesAsync(string driverId)
        {
            return await _rideRepository.GetRidesByDriverAsync(driverId);
        }

        public async Task<IEnumerable<Ride>> SearchRidesAsync(string fromLocation, string toLocation, DateTime? startTime)
        {
            return await _rideRepository.SearchRidesAsync(fromLocation, toLocation, startTime);
        }

        public async Task<Ride> CreateRideAsync(Ride ride)
        {
            ride.Status = "Active";
            ride.CreatedAt = DateTime.UtcNow;
            return await _rideRepository.CreateAsync(ride);
        }

        public async Task<Ride> UpdateRideAsync(Ride ride)
        {
            return await _rideRepository.UpdateAsync(ride);
        }

        public async Task<bool> DeleteRideAsync(int rideId)
        {
            return await _rideRepository.DeleteAsync(rideId);
        }

        public async Task<bool> CancelRideAsync(int rideId)
        {
            return await _rideRepository.UpdateRideStatusAsync(rideId, "Cancelled");
        }

        public async Task<bool> CompleteRideAsync(int rideId)
        {
            return await _rideRepository.UpdateRideStatusAsync(rideId, "Completed");
        }
    }
}
