namespace JobPortalAPI.Models
{
    public class AuthModel
    {
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }   // Default role
    }
}
