using System.Text.Json.Serialization;

namespace JobPortalAPI.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string JobType { get; set; } = "Full-Time";
        public decimal Salary { get; set; }
        public DateTime PostedOn { get; set; } = DateTime.UtcNow;

        public string RecruiterId { get; set; } = string.Empty;
        [JsonIgnore]
        public User? Recruiter { get; set; }

        public int CompanyId { get; set; }
        [JsonIgnore]
        public Company? Company { get; set; } 
    }
}
