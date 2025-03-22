using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TestProject.Controllers;

namespace TestProject.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }

        // [Required]
        public string DriversId { get; set; } = string.Empty;
        [ForeignKey(nameof(DriversId))]
        public ApplicationUser? Driver { get; set; }

        // [Required]
        public string StartPosition { get; set; }

        //  [Required]
        public string Destination { get; set; }

        //  [Required]
        public DateTime DepartureTime { get; set; }

        // [Required(ErrorMessage = "zaduljitelno pole")]
        public DateTime ReturnTime { get; set; }

        // [Required]
        public int Price { get; set; }

        // [Required]
        public int TotalSeats { get; set; }

        public int FreeSeats { get; set; }

        //   [Required]
        public string CarModel { get; set; }

        //  [Required]
        public string PlateNumber { get; set; }

        public string? ImagePath { get; set; }

        public TripStatus StatusTrip { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsRecurring { get; set; }
        public String? RecurrenceInterval { get; set; } = "00:00:00";
        public DateTime? NextRunDate { get; set; } = DateTime.Now;

        public ICollection<TripParticipant>? TripParticipants { get; set; } 
        public ICollection<Request>? Requests { get; set; } 
        public ICollection<Rating>? Ratings { get; set; } 
    }
}
