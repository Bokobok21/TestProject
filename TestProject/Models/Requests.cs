using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Requests
    {
        [Key]
        public int Id { get; set; }
        public required int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trips Trip { get; set; } = null!;

        public required string RequesterId { get; set; }
        [ForeignKey(nameof(RequesterId))]
        public Users Requester { get; set; } = null!;
        public Status Status { get; set; }
        public DateTime Date { get; set; }

    }
}
