using Pikmi.API.Entities;

namespace Pikmi.API.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating?> GetByIdAsync(int ratingId);
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<IEnumerable<Rating>> GetRatingsByUserAsync(string userId);
        Task<IEnumerable<Rating>> GetRatingsByRideAsync(int rideId);
        Task<Rating> CreateAsync(Rating rating);
        Task<Rating> UpdateAsync(Rating rating);
        Task<bool> DeleteAsync(int ratingId);
        Task<double> GetAverageRatingForUserAsync(string userId);
        Task<bool> HasUserRatedRideAsync(int rideId, string raterId);
    }
}
