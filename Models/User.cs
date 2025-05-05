// User.cs
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace JobPortalAPI.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        
        // Refresh Token Support
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Email Verification
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        [JsonIgnore]
        public List<JobApplication>? Applications { get; set; }

        [JsonIgnore]
        public List<Job>? PostedJobs { get; set; }

        [JsonIgnore]
        public List<Company> Companies { get; set; } = new List<Company>();

        [JsonIgnore]
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}