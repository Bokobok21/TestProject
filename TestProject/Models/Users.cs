using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace TestProject.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Position { get; set; } = string.Empty;

        public List<Trips>? Trips { get; set; }
    }
}
