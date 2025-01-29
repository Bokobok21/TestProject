using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } = null!;

        [Required]
        public string ReviewerId { get; set; } = null!;
        [ForeignKey(nameof(ReviewerId))]
        public ApplicationUser Reviewer { get; set; } = null!;

        [Required]
        public int Score { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime Date { get; set; }

      //  public ICollection<Request>? Requests { get; set; }
    }
}
