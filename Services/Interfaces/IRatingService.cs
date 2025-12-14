using Pikmi.API.Entities;

namespace Pikmi.API.Services.Interfaces
{
    public interface IRatingService
    {
        Task<Rating?> GetRatingByIdAsync(int ratingId);
        Task<IEnumerable<Rating>> GetAllRatingsAsync();
        Task<IEnumerable<Rating>> GetUserRatingsAsync(string userId);
        Task<IEnumerable<Rating>> GetRideRatingsAsync(int rideId);
        Task<Rating> CreateRatingAsync(int rideId, string raterId, string ratedUserId, int ratingValue, string comment);
        Task<Rating> UpdateRatingAsync(Rating rating);
        Task<bool> DeleteRatingAsync(int ratingId);
        Task<double> GetUserAverageRatingAsync(string userId);
    }
}
