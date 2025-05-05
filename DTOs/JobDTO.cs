namespace JobPortalAPI.DTOs
{
    public class JobDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string JobType { get; set; } = "Full-Time";
        public string CompanyName { get; set; } = string.Empty; // ✅ Add this
    }
}
