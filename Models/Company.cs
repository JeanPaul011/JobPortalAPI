using System.Text.Json.Serialization;

namespace JobPortalAPI.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Job>? Jobs { get; set; }

        [JsonIgnore]
        public List<Review>? Reviews { get; set; }

        [JsonIgnore]
        public List<User> Recruiters { get; set; } = new List<User>();  
    }
}
