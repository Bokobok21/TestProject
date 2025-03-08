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

    }
}
