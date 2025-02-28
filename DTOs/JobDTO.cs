namespace JobPortalAPI.DTOs
{
    public class JobDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string JobType { get; set; } = "Full-Time";
        public decimal Salary { get; set; }
        public int CompanyId { get; set; }
    }
}
