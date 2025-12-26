using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pikmi.API.DTOs.Ride;
using Pikmi.API.Entities;
using Pikmi.API.Services.Interfaces;
using System.Security.Claims;

namespace Pikmi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RidesController : ControllerBase
    {
        private readonly IRideService _rideService;
        private readonly IMapper _mapper;

        public RidesController(IRideService rideService, IMapper mapper)
        {
            _rideService = rideService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RideResponseDto>>> GetAllRides()
        {
            var rides = await _rideService.GetAllRidesAsync();
            var rideDtos = _mapper.Map<IEnumerable<RideResponseDto>>(rides);
            return Ok(rideDtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<RideResponseDto>>> GetActiveRides()
        {
            var rides = await _rideService.GetActiveRidesAsync();
            var rideDtos = _mapper.Map<IEnumerable<RideResponseDto>>(rides);
            return Ok(rideDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RideResponseDto>> GetRide(int id)
        {
            var ride = await _rideService.GetRideByIdAsync(id);

            if (ride == null)
                return NotFound(new { message = "Ride not found" });

            var rideDto = _mapper.Map<RideResponseDto>(ride);
            return Ok(rideDto);
        }

        [HttpGet("my-rides")]
        public async Task<ActionResult<IEnumerable<RideResponseDto>>> GetMyRides()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rides = await _rideService.GetDriverRidesAsync(userId);
            var rideDtos = _mapper.Map<IEnumerable<RideResponseDto>>(rides);
            return Ok(rideDtos);
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<RideResponseDto>>> SearchRides([FromBody] SearchRideDto dto)
        {
            var rides = await _rideService.SearchRidesAsync(dto.FromLocation, dto.ToLocation, dto.StartTime);
            var rideDtos = _mapper.Map<IEnumerable<RideResponseDto>>(rides);
            return Ok(rideDtos);
        }

        [HttpPost]
        public async Task<ActionResult<RideResponseDto>> CreateRide([FromBody] CreateRideDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ride = _mapper.Map<Ride>(dto);
            ride.DriverId = userId;

            var createdRide = await _rideService.CreateRideAsync(ride);
            var rideDto = _mapper.Map<RideResponseDto>(createdRide);

            return CreatedAtAction(nameof(GetRide), new { id = createdRide.RideId }, rideDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RideResponseDto>> UpdateRide(int id, [FromBody] UpdateRideDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ride = await _rideService.GetRideByIdAsync(id);

            if (ride == null)
                return NotFound(new { message = "Ride not found" });

            if (ride.DriverId != userId)
                return Forbid();

            if (!string.IsNullOrEmpty(dto.FromLocation))
                ride.FromLocation = dto.FromLocation;

            if (!string.IsNullOrEmpty(dto.ToLocation))
                ride.ToLocation = dto.ToLocation;

            if (dto.StartTime.HasValue)
                ride.StartTime = dto.StartTime.Value;

            if (dto.SeatsAvailable.HasValue)
                ride.SeatsAvailable = dto.SeatsAvailable.Value;

            if (dto.CostInCoins.HasValue)
                ride.CostInCoins = dto.CostInCoins.Value;

            var updatedRide = await _rideService.UpdateRideAsync(ride);
            var rideDto = _mapper.Map<RideResponseDto>(updatedRide);

            return Ok(rideDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRide(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ride = await _rideService.GetRideByIdAsync(id);

            if (ride == null)
                return NotFound(new { message = "Ride not found" });

            if (ride.DriverId != userId)
                return Forbid();

            await _rideService.DeleteRideAsync(id);

            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelRide(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ride = await _rideService.GetRideByIdAsync(id);

            if (ride == null)
                return NotFound(new { message = "Ride not found" });

            if (ride.DriverId != userId)
                return Forbid();

            await _rideService.CancelRideAsync(id);

            return Ok(new { message = "Ride cancelled successfully" });
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteRide(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ride = await _rideService.GetRideByIdAsync(id);

            if (ride == null)
                return NotFound(new { message = "Ride not found" });

            if (ride.DriverId != userId)
                return Forbid();

            await _rideService.CompleteRideAsync(id);

            return Ok(new { message = "Ride completed successfully" });
        }
    }
}
