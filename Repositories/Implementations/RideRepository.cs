using Microsoft.EntityFrameworkCore;
using Pikmi.API.Data;
using Pikmi.API.Entities;
using Pikmi.API.Repositories.Interfaces;

namespace Pikmi.API.Repositories.Implementations
{
    public class RideRepository : IRideRepository
    {
        private readonly ApplicationDbContext _context;

        public RideRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ride?> GetByIdAsync(int rideId)
        {
            return await _context.Rides
                .Include(r => r.Driver)
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.RideId == rideId);
        }

        public async Task<IEnumerable<Ride>> GetAllAsync()
        {
            return await _context.Rides
                .Include(r => r.Driver)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ride>> GetActiveRidesAsync()
        {
            return await _context.Rides
                .Include(r => r.Driver)
                .Where(r => r.Status == "Active" && r.StartTime > DateTime.UtcNow && r.SeatsAvailable > 0)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ride>> GetRidesByDriverAsync(string driverId)
        {
            return await _context.Rides
                .Include(r => r.Bookings)
                .Where(r => r.DriverId == driverId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ride>> SearchRidesAsync(string fromLocation, string toLocation, DateTime? startTime)
        {
            var query = _context.Rides
                .Include(r => r.Driver)
                .Where(r => r.Status == "Active" && r.SeatsAvailable > 0);

            if (!string.IsNullOrEmpty(fromLocation))
            {
                query = query.Where(r => r.FromLocation.Contains(fromLocation));
            }

            if (!string.IsNullOrEmpty(toLocation))
            {
                query = query.Where(r => r.ToLocation.Contains(toLocation));
            }

            if (startTime.HasValue)
            {
                var startOfDay = startTime.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(r => r.StartTime >= startOfDay && r.StartTime < endOfDay);
            }

            return await query.OrderBy(r => r.StartTime).ToListAsync();
        }

        public async Task<Ride> CreateAsync(Ride ride)
        {
            await _context.Rides.AddAsync(ride);
            await _context.SaveChangesAsync();
            return ride;
        }

        public async Task<Ride> UpdateAsync(Ride ride)
        {
            _context.Rides.Update(ride);
            await _context.SaveChangesAsync();
            return ride;
        }

        public async Task<bool> DeleteAsync(int rideId)
        {
            var ride = await _context.Rides.FindAsync(rideId);
            if (ride == null) return false;

            _context.Rides.Remove(ride);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRideStatusAsync(int rideId, string status)
        {
            var ride = await _context.Rides.FindAsync(rideId);
            if (ride == null) return false;

            ride.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
