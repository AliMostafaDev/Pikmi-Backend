using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pikmi.API.DTOs.Booking;
using Pikmi.API.Services.Interfaces;
using System.Security.Claims;

namespace Pikmi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingsController(IBookingService bookingService, IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            var bookingDtos = _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
            return Ok(bookingDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.PassengerId != userId && booking.Ride.DriverId != userId)
                return Forbid();

            var bookingDto = _mapper.Map<BookingResponseDto>(booking);
            return Ok(bookingDto);
        }

        [HttpGet("my-bookings")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _bookingService.GetPassengerBookingsAsync(userId);
            var bookingDtos = _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
            return Ok(bookingDtos);
        }

        [HttpGet("ride/{rideId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetRideBookings(int rideId)
        {
            var bookings = await _bookingService.GetRideBookingsAsync(rideId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var firstBooking = bookings.FirstOrDefault();
            if (firstBooking != null && firstBooking.Ride.DriverId != userId)
                return Forbid();

            var bookingDtos = _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
            return Ok(bookingDtos);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var booking = await _bookingService.CreateBookingAsync(dto.RideId, userId, dto.SeatsBooked);
                var bookingDto = _mapper.Map<BookingResponseDto>(booking);

                return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, bookingDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.Ride.DriverId != userId)
                return Forbid();

            await _bookingService.ConfirmBookingAsync(id);

            return Ok(new { message = "Booking confirmed successfully" });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.PassengerId != userId && booking.Ride.DriverId != userId)
                return Forbid();

            await _bookingService.CancelBookingAsync(id);

            return Ok(new { message = "Booking cancelled and coins refunded" });
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.Ride.DriverId != userId)
                return Forbid();

            await _bookingService.CompleteBookingAsync(id);

            return Ok(new { message = "Booking completed and driver paid" });
        }
    }
}
