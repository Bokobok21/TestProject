using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class TripParticipant
    {
        public string ParticipantId { get; set; }
        [ForeignKey(nameof(ParticipantId))]
        public ApplicationUser Participant { get; set; }

        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; }
    }
}
