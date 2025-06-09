using EventTrackingVer2.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventTrackingVer2.Models
{
    public class Event
    {

        [Key]
        public int EventId { get; set; }

        public string? EventName { get; set; }


        public string? Organizer { get; set; }


        public string? Location { get; set; }

        [Required]
        public DateTime? StartTime { get; set; }

        [Required]
        public DateTime? EndTime { get; set; }

        [Required]
        public int NumberOfParticipants { get; set; }

        public string? UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; } = default!;

        public ICollection<Guest>? Guests { get; set; } = default!;
    }
}
