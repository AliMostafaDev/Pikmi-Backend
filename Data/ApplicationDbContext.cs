using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pikmi.API.Entities;

namespace Pikmi.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ride> Rides { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ride>()
                .HasOne(r => r.Driver)
                .WithMany(u => u.DriverRides)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Booking>()
                .HasOne(b => b.Passenger)
                .WithMany(u => u.PassengerBookings)
                .HasForeignKey(b => b.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Ride)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RideId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<Rating>()
                .HasOne(r => r.Ride)
                .WithMany(ride => ride.Ratings)
                .HasForeignKey(r => r.RideId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .HasOne(r => r.Rater)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(r => r.RaterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Rating>()
                .HasOne(r => r.RatedUser)
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(r => r.RatedUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
    
}
