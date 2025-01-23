using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public required int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trips Trip { get; set; } = null!;
        public required string ReviewerId { get; set; }
        [ForeignKey(nameof(ReviewerId))]
        public Users Reviewer { get; set; } = null!;
        public double Score { get; set; }
        public required string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
