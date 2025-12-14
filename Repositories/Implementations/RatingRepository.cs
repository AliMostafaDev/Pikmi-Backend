using Microsoft.EntityFrameworkCore;
using Pikmi.API.Data;
using Pikmi.API.Entities;
using Pikmi.API.Repositories.Interfaces;

namespace Pikmi.API.Repositories.Implementations
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Rating?> GetByIdAsync(int ratingId)
        {
            return await _context.Ratings
                .Include(r => r.Ride)
                .Include(r => r.Rater)
                .Include(r => r.RatedUser)
                .FirstOrDefaultAsync(r => r.RatingId == ratingId);
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            return await _context.Ratings
                .Include(r => r.Ride)
                .Include(r => r.Rater)
                .Include(r => r.RatedUser)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rating>> GetRatingsByUserAsync(string userId)
        {
            return await _context.Ratings
                .Include(r => r.Ride)
                .Include(r => r.Rater)
                .Where(r => r.RatedUserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rating>> GetRatingsByRideAsync(int rideId)
        {
            return await _context.Ratings
                .Include(r => r.Rater)
                .Include(r => r.RatedUser)
                .Where(r => r.RideId == rideId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Rating> CreateAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task<Rating> UpdateAsync(Rating rating)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task<bool> DeleteAsync(int ratingId)
        {
            var rating = await _context.Ratings.FindAsync(ratingId);
            if (rating == null) return false;

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetAverageRatingForUserAsync(string userId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.RatedUserId == userId)
                .ToListAsync();

            return ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;
        }

        public async Task<bool> HasUserRatedRideAsync(int rideId, string raterId)
        {
            return await _context.Ratings
                .AnyAsync(r => r.RideId == rideId && r.RaterId == raterId);
        }
    }
}
