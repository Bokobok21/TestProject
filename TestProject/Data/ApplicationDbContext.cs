using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TestProject.Models;

namespace TestProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //adds unique constraint of tripParticipant
            modelBuilder.Entity<TripParticipant>()
                .HasIndex(tp => new { tp.UserId, tp.TripId })
                .IsUnique();

            //fixes the multiple cascade paths for rating
            modelBuilder.Entity<Rating>()
               .HasOne(r => r.Trip)
               .WithMany(r => r.Ratings)
               .HasForeignKey(r => r.TripId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(r => r.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //fixes the multiple cascade paths for request
            modelBuilder.Entity<Request>()
               .HasOne(r => r.Trip)
               .WithMany(r => r.Requests)
               .HasForeignKey(r => r.TripId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany(r => r.Requests)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

           // fixes the multiple cascade paths for tripParticipant
            modelBuilder.Entity<TripParticipant>()
                 .HasOne(tp => tp.Trip)
                 .WithMany(x => x.TripParticipants) 
                 .HasForeignKey(tp => tp.TripId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TripParticipant>()
                .HasOne(tp => tp.User)
                .WithMany(x => x.TripParticipants) 
                .HasForeignKey(tp => tp.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RequestDriver> RequestDrivers { get; set; }
        public DbSet<TripParticipant> TripParticipants { get; set; }

    }
}