using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class TripParticipant
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }  // Add null-forgiving operator

        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; }  // Add null-forgiving operator

        [Range(1, int.MaxValue, ErrorMessage = "Броят места трябва да бъде поне 1")]
        public int NumberOfSeats { get; set; } = 1; // Default to 1 seat
    }
}
