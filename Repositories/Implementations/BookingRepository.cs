using Microsoft.EntityFrameworkCore;
using Pikmi.API.Data;
using Pikmi.API.Entities;
using Pikmi.API.Repositories.Interfaces;

namespace Pikmi.API.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Ride)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Ride)
                .Include(b => b.Passenger)
                .OrderByDescending(b => b.BookedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByPassengerAsync(string passengerId)
        {
            return await _context.Bookings
                .Include(b => b.Ride)
                    .ThenInclude(r => r.Driver)
                .Where(b => b.PassengerId == passengerId)
                .OrderByDescending(b => b.BookedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRideAsync(int rideId)
        {
            return await _context.Bookings
                .Include(b => b.Passenger)
                .Where(b => b.RideId == rideId)
                .OrderBy(b => b.BookedAt)
                .ToListAsync();
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BookingExistsAsync(int rideId, string passengerId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.RideId == rideId && b.PassengerId == passengerId && b.Status != "Cancelled");
        }
    }
}
