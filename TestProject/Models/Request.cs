using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace TestProject.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

       // [Required]
        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } 

     //   [Required]
        public string RequesterId { get; set; }
        [ForeignKey(nameof(RequesterId))]
        public ApplicationUser Requester { get; set; } 

        public RequestStatus StatusRequest { get; set; }

        public DateTime Date { get; set; }
  
    }
}
