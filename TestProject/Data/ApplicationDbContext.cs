using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestProject.Models;

namespace TestProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Users> Users { get; set; } 
        public DbSet<Trips> Trips { get; set; } 
        public DbSet<Requests> Requests { get; set; } 
        public DbSet<Rating> Ratings { get; set; } 

    }
}