using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class Trips
    {
        public int Id { get; set; }
        public string DriversId { get; set; } = string.Empty;
        [ForeignKey(nameof(DriversId))]
        public Users Driver { get; set; } = null!;
        public required string StartPosition { get; set; } 
        public required string EndPosition { get; set; }
        public DateTime DateAndTime { get; set; }
        public int Price { get; set; }

        public int RatingId { get; set; }
        [ForeignKey(nameof(RatingId))]
        public Rating? Rating { get; set; }
        public int FreeSeats { get; set; }
    }
}
