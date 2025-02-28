namespace JobPortalAPI.DTOs
{
    public class JobApplicationDTO
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
    }
}
