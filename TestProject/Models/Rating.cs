using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public required string TripId { get; set; }
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
