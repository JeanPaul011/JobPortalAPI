// Models/AdminRequest.cs
namespace JobPortalAPI.Models
{
    public class AdminRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    }
}
