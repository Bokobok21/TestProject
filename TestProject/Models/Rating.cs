using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

     //   [Required]
        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } 

       // [Required]
        public string ReviewerId { get; set; } 
        [ForeignKey(nameof(ReviewerId))]
        public ApplicationUser Reviewer { get; set; } 

       // [Required]
        public int Score { get; set; }

       // [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime Date { get; set; }

    }
}
