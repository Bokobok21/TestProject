using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        public string FirstName { get; set; } = string.Empty;

      //  [Required]
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        public string? ImagePath { get; set; }
        public DateTime? DateOfDriverAcceptance { get; set; }

        public ICollection<TripParticipant>? TripParticipants { get; set; }
        public ICollection<Request>? Requests { get; set; } 
        public ICollection<RequestDriver>? RequestDrivers { get; set; } 
        public ICollection<Rating>? Ratings { get; set; } 
    }
}
