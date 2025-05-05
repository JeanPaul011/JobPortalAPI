using System.Text.Json.Serialization;
using System;

namespace JobPortalAPI.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        // Foreign key to the Job being applied to
        public int JobId { get; set; }

        // Foreign key to the JobSeeker (User)
        public string JobSeekerId { get; set; } = string.Empty;

        // Application message (optional)
        public string? Message { get; set; }

        // Application status (default is Pending)
        public string Status { get; set; } = "Pending"; // Valid values: Pending, Accepted, Rejected, etc.

        // Timestamp of when the application was submitted
        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        public Job? Job { get; set; }

        [JsonIgnore]
        public User? JobSeeker { get; set; }
    }
}
