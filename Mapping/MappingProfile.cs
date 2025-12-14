using AutoMapper;
using Pikmi.API.DTOs.Balance;
using Pikmi.API.DTOs.Booking;
using Pikmi.API.DTOs.Identity;
using Pikmi.API.DTOs.Rating;
using Pikmi.API.DTOs.Ride;
using Pikmi.API.DTOs.User;
using Pikmi.API.Entities;

namespace Pikmi.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ApplicationUser, UserDto>();

            CreateMap<CreateUserDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateUserDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));



            CreateMap<CreateRideDto, Ride>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Ride, RideResponseDto>()
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => $"{src.Driver.FirstName} {src.Driver.LastName}"))
                .ForMember(dest => dest.DriverProfileImage, opt => opt.MapFrom(src => src.Driver.ProfileImage))
                .ForMember(dest => dest.DriverRating, opt => opt.MapFrom(src => src.Driver.AverageRating))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.Bookings.Count));



            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"))
                .ForMember(dest => dest.BookedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.PassengerName, opt => opt.MapFrom(src => $"{src.Passenger.FirstName} {src.Passenger.LastName}"))
                .ForMember(dest => dest.PassengerProfileImage, opt => opt.MapFrom(src => src.Passenger.ProfileImage))
                .ForMember(dest => dest.FromLocation, opt => opt.MapFrom(src => src.Ride.FromLocation))
                .ForMember(dest => dest.ToLocation, opt => opt.MapFrom(src => src.Ride.ToLocation))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Ride.StartTime))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => $"{src.Ride.Driver.FirstName} {src.Ride.Driver.LastName}"))
                .ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => src.Ride.DriverId));



            CreateMap<CreateRatingDto, Rating>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Rating, RatingResponseDto>()
                .ForMember(dest => dest.RaterName, opt => opt.MapFrom(src => $"{src.Rater.FirstName} {src.Rater.LastName}"))
                .ForMember(dest => dest.RaterProfileImage, opt => opt.MapFrom(src => src.Rater.ProfileImage))
                .ForMember(dest => dest.RatedUserName, opt => opt.MapFrom(src => $"{src.RatedUser.FirstName} {src.RatedUser.LastName}"));



            CreateMap<ApplicationUser, BalanceResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
