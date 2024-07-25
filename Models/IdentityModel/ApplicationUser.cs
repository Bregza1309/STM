using Microsoft.AspNetCore.Identity;

namespace StudentTransportManagement_STM_.Shared.IdentityModel
{
    public class ApplicationUser:IdentityUser
    {
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Avatar { get; set; } = "profile.png";
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
    }
}
