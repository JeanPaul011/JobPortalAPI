namespace JobPortalAPI.Models
{
    public class SavedJob
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobSeekerId { get; set; } = string.Empty;
    }
}
