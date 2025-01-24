using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Trips
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DriversId { get; set; } = null!;
        [ForeignKey(nameof(DriversId))]
        public ApplicationUser Driver { get; set; } = null!;

        [Required]
        public string StartPosition { get; set; } = null!;

        [Required]
        public string Destination { get; set; } = null!;

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ReturnTime { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int FreeSeats { get; set; }

        [Required]
        public string CarModel { get; set; } = null!;

        [Required]
        public string PlateNumber { get; set; } = null!;

        public TripStatus StatusTrip { get; set; } 

        public int? RatingId { get; set; } 
        [ForeignKey(nameof(RatingId))]

        public Rating? Rating { get; set; }

        public ICollection<Requests>? Requests { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
    }
}
