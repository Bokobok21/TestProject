using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestProject.Models;

namespace TestProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } 
        public DbSet<Trip> Trips { get; set; } 
        public DbSet<Request> Requests { get; set; } 
        public DbSet<Rating> Ratings { get; set; } 
        public DbSet<RequestDriver> RequestDrivers { get; set; }
        public DbSet<TripParticipant> TripParticipants { get; set; }

    }
}