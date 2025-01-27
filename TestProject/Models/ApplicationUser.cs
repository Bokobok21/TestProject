using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = string.Empty;

        public ICollection<Trip>? Trips { get; set; }
        public ICollection<Request>? Requests { get; set; }
        public ICollection<Rating>? Ratings { get; set; } 
    }
}
