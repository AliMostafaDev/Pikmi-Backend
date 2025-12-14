using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pikmi.API.DTOs.Rating;
using Pikmi.API.Services.Interfaces;
using System.Security.Claims;

namespace Pikmi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IMapper _mapper;

        public RatingsController(IRatingService ratingService, IMapper mapper)
        {
            _ratingService = ratingService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetAllRatings()
        {
            var ratings = await _ratingService.GetAllRatingsAsync();
            var ratingDtos = _mapper.Map<IEnumerable<RatingResponseDto>>(ratings);
            return Ok(ratingDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RatingResponseDto>> GetRating(int id)
        {
            var rating = await _ratingService.GetRatingByIdAsync(id);

            if (rating == null)
                return NotFound(new { message = "Rating not found" });

            var ratingDto = _mapper.Map<RatingResponseDto>(rating);
            return Ok(ratingDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetUserRatings(string userId)
        {
            var ratings = await _ratingService.GetUserRatingsAsync(userId);
            var ratingDtos = _mapper.Map<IEnumerable<RatingResponseDto>>(ratings);
            return Ok(ratingDtos);
        }

        [HttpGet("ride/{rideId}")]
        public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetRideRatings(int rideId)
        {
            var ratings = await _ratingService.GetRideRatingsAsync(rideId);
            var ratingDtos = _mapper.Map<IEnumerable<RatingResponseDto>>(ratings);
            return Ok(ratingDtos);
        }

        [HttpGet("user/{userId}/average")]
        public async Task<ActionResult<double>> GetUserAverageRating(string userId)
        {
            var average = await _ratingService.GetUserAverageRatingAsync(userId);
            return Ok(average);
        }

        [HttpPost]
        public async Task<ActionResult<RatingResponseDto>> CreateRating([FromBody] CreateRatingDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var rating = await _ratingService.CreateRatingAsync(
                    dto.RideId,
                    userId,
                    dto.RatedUserId,
                    dto.RatingValue,
                    dto.Comment
                );

                var ratingDto = _mapper.Map<RatingResponseDto>(rating);

                return CreatedAtAction(nameof(GetRating), new { id = rating.RatingId }, ratingDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _ratingService.GetRatingByIdAsync(id);
            if (rating == null)
                return NotFound(new { message = "Rating not found" });

            // Check if user is the rater or admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            if (rating.RaterId != userId && !isAdmin)
                return Forbid();

            await _ratingService.DeleteRatingAsync(id);

            return NoContent();
        }
    }
}
