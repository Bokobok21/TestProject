using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } = null!;

        [Required]
        public string RequesterId { get; set; } = null!;
        [ForeignKey(nameof(RequesterId))]
        public ApplicationUser Requester { get; set; } = null!;

        public RequestStatus StatusRequest { get; set; }

        public DateTime Date { get; set; }

    }
}
