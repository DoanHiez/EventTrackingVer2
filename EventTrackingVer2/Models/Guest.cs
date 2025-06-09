using System.ComponentModel.DataAnnotations;
using EventTrackingVer2.Data;

namespace EventTrackingVer2.Models
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }
        public string GuestCode { get; set; } = "";
        public string GuestName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Status { get; set; } = "Unconfirm";
        public string Result { get; set; } = "Absent";

        public int? EventId { get; set; }
        public Event? Event { get; set; } = default!;
    }
}
