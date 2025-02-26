using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace JobPortalAPI.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "JobSeeker"; // "Recruiter" or "JobSeeker"
        
        [JsonIgnore]
        public List<JobApplication>? Applications { get; set; }

        [JsonIgnore]
        public List<Job>? PostedJobs { get; set; } // Only for recruiters

         [JsonIgnore]
        public List<Company> Companies { get; set; } = new List<Company>();  // ✅ Added (Recruiters can manage multiple companies)

        [JsonIgnore]
        public List<Review> Reviews { get; set; } = new List<Review>();  // ✅ Added (Users can leave reviews)
    }
}
