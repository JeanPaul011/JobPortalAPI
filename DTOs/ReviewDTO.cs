namespace JobPortalAPI.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
