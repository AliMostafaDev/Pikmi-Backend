using Microsoft.AspNetCore.Identity;
using Pikmi.API.Entities;
using Pikmi.API.Repositories.Interfaces;
using Pikmi.API.Services.Interfaces;

namespace Pikmi.API.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingService(
            IRatingRepository ratingRepository,
            UserManager<ApplicationUser> userManager)
        {
            _ratingRepository = ratingRepository;
            _userManager = userManager;
        }

        public async Task<Rating?> GetRatingByIdAsync(int ratingId)
        {
            return await _ratingRepository.GetByIdAsync(ratingId);
        }

        public async Task<IEnumerable<Rating>> GetAllRatingsAsync()
        {
            return await _ratingRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Rating>> GetUserRatingsAsync(string userId)
        {
            return await _ratingRepository.GetRatingsByUserAsync(userId);
        }

        public async Task<IEnumerable<Rating>> GetRideRatingsAsync(int rideId)
        {
            return await _ratingRepository.GetRatingsByRideAsync(rideId);
        }

        public async Task<Rating> CreateRatingAsync(int rideId, string raterId, string ratedUserId, int ratingValue, string comment)
        {
            if (await _ratingRepository.HasUserRatedRideAsync(rideId, raterId))
                throw new Exception("You have already rated this ride");

            if (ratingValue < 1 || ratingValue > 5)
                throw new Exception("Rating must be between 1 and 5");

            var rating = new Rating
            {
                RideId = rideId,
                RaterId = raterId,
                RatedUserId = ratedUserId,
                RatingValue = ratingValue,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            var createdRating = await _ratingRepository.CreateAsync(rating);

            await UpdateUserAverageRatingAsync(ratedUserId);

            return createdRating;
        }

        public async Task<Rating> UpdateRatingAsync(Rating rating)
        {
            var updatedRating = await _ratingRepository.UpdateAsync(rating);
            await UpdateUserAverageRatingAsync(rating.RatedUserId);
            return updatedRating;
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
                return false;

            var result = await _ratingRepository.DeleteAsync(ratingId);
            await UpdateUserAverageRatingAsync(rating.RatedUserId);
            return result;
        }

        public async Task<double> GetUserAverageRatingAsync(string userId)
        {
            return await _ratingRepository.GetAverageRatingForUserAsync(userId);
        }

        private async Task UpdateUserAverageRatingAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.AverageRating = await _ratingRepository.GetAverageRatingForUserAsync(userId);
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
