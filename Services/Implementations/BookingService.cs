using Pikmi.API.Entities;
using Pikmi.API.Repositories.Interfaces;
using Pikmi.API.Services.Interfaces;

namespace Pikmi.API.Services.Implementations
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRideRepository _rideRepository;
        private readonly IWalletService _walletService;

        public BookingService(
            IBookingRepository bookingRepository,
            IRideRepository rideRepository,
            IWalletService walletService)
        {
            _bookingRepository = bookingRepository;
            _rideRepository = rideRepository;
            _walletService = walletService;
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingRepository.GetByIdAsync(bookingId);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Booking>> GetPassengerBookingsAsync(string passengerId)
        {
            return await _bookingRepository.GetBookingsByPassengerAsync(passengerId);
        }

        public async Task<IEnumerable<Booking>> GetRideBookingsAsync(int rideId)
        {
            return await _bookingRepository.GetBookingsByRideAsync(rideId);
        }

        public async Task<Booking> CreateBookingAsync(int rideId, string passengerId, int seatsBooked)
        {
            var ride = await _rideRepository.GetByIdAsync(rideId);
            if (ride == null)
                throw new Exception("Ride not found");

            if (ride.Status != "Active")
                throw new Exception("Ride is not active");

            if (ride.SeatsAvailable < seatsBooked)
                throw new Exception("Not enough seats available");

            if (await _bookingRepository.BookingExistsAsync(rideId, passengerId))
                throw new Exception("You have already booked this ride");

            // Calculate cost
            decimal totalCost = ride.CostInCoins * seatsBooked;

            if (!await _walletService.HasSufficientBalanceAsync(passengerId, totalCost))
                throw new Exception("Insufficient balance");

            await _walletService.DeductBalanceAsync(passengerId, totalCost);

            ride.SeatsAvailable -= seatsBooked;
            await _rideRepository.UpdateAsync(ride);

            var booking = new Booking
            {
                RideId = rideId,
                PassengerId = passengerId,
                SeatsBooked = seatsBooked,
                CoinsUsed = totalCost,
                Status = "Pending",
                BookedAt = DateTime.UtcNow
            };

            return await _bookingRepository.CreateAsync(booking);
        }

        public async Task<bool> ConfirmBookingAsync(int bookingId)
        {
            return await _bookingRepository.UpdateBookingStatusAsync(bookingId, "Confirmed");
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return false;

            await _walletService.AddBalanceAsync(booking.PassengerId, booking.CoinsUsed);

            var ride = await _rideRepository.GetByIdAsync(booking.RideId);
            if (ride != null)
            {
                ride.SeatsAvailable += booking.SeatsBooked;
                await _rideRepository.UpdateAsync(ride);
            }

            return await _bookingRepository.UpdateBookingStatusAsync(bookingId, "Cancelled");
        }

        public async Task<bool> CompleteBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return false;

            var ride = await _rideRepository.GetByIdAsync(booking.RideId);
            await _walletService.AddBalanceAsync(ride.DriverId, booking.CoinsUsed);

            return await _bookingRepository.UpdateBookingStatusAsync(bookingId, "Completed");
        }
    }
}
