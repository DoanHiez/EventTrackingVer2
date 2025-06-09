using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventTrackingVer2.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? Sex { get; set; }
        public string? Address { get; set; }

        [NotMapped]
        public string SelectedRole { get; set; } = "";
    }

}
