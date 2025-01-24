using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        public List<Trips>? Trips { get; set; }
    }
}
