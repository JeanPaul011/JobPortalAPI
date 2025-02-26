using System.Text.Json.Serialization;

namespace JobPortalAPI.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobSeekerId { get; set; } = string.Empty;

        [JsonIgnore]
        public Job? Job { get; set; }
        
        [JsonIgnore]
        public User? JobSeeker { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected
        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
    }
}
