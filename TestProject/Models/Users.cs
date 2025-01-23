using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace TestProject.Models
{
    public class Users : IdentityUser
    {
        public string Position { get; set; } = string.Empty;

        public List<Trips>? Trips { get; set; }
    }
}
