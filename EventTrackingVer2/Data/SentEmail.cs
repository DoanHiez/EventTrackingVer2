using System;
using System.ComponentModel.DataAnnotations;

namespace EventTrackingVer2.Data
{
    public class SentEmail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string ToEmail { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }
    }
}
