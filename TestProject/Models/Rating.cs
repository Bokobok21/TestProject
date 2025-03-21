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
        public string UserId { get; set; } 
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        // [Required]
        [Range(1, 5)]
        public int Score { get; set; }

       // [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

    }
}
