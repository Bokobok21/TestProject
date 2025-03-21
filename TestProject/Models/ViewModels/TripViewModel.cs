using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models.ViewModels
{
    public class TripViewModel : Trip
    {
        [NotMapped]
       // [Required(ErrorMessage = "Please upload an image.")]
        public IFormFile? ImageFile { get; set; }
        public string? DriverName { get; set; }

        public int? RecurrenceIntervalMinutes { get; set; } = 0;
        public int? RecurrenceIntervalDays { get; set; } = 0;
        public int? RecurrenceIntervalHours { get; set; } = 0;

        //public bool CanReview { get; set; } = false;
        //public RatingViewModel? Review { get; set; } 

    }
}
