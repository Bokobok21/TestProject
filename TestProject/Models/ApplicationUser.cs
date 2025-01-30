using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = string.Empty;

        public ICollection<Trip>? Trips { get; set; }
        public ICollection<Request>? Requests { get; set; } 
       // public ICollection<RequestDriver>? RequestDrivers { get; set; } 
        public ICollection<Rating>? Ratings { get; set; } 
    }
}
//i have four tabbles/entitie/models iin asp net core i have applicationuser and trip and review and request and i have admin/moderator/tourist roles. only drivers can make a trip and when you first register you become tourist but you can apply if you want to be a driver and there youe enter the info for the cars you have and their plate numbers. so this probably mmeans i need to make another table cars. the drivers has the option to add more cars later if he wants. 
// kak shte se integrira service/Iservice, viewModel, partiaView
// izpolzvai DisplayFor and EditFor
// v0 i codepen

