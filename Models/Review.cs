using System.Text.Json.Serialization;

namespace JobPortalAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int Rating { get; set; } // Rating from 1 to 5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; } = null!;

        [JsonIgnore]
        public Company Company { get; set; } = null!;
    }
}
