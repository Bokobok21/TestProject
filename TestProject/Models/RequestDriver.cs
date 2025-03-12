using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class RequestDriver
    {
        [Key]
        public int Id { get; set; }

       // [Required]
        public string UserId { get; set; } 
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } 

        public RequestStatus StatusRequest { get; set; }

        public DateTime Date { get; set; }
    }
}   
