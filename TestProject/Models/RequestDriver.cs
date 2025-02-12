using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class RequestDriver
    {
        [Key]
        public int Id { get; set; }

       // [Required]
        public string RequesterId { get; set; } = null!;
        [ForeignKey(nameof(RequesterId))]
        public ApplicationUser Requester { get; set; } = null!;

        public string ImagePath { get; set; }

        public RequestStatus StatusRequest { get; set; }

        public DateTime? DateOfAcceptance { get; set; }
    }
}
