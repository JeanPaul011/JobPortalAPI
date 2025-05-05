using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.DTOs
{
    public class RegisterUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required] // Ensure it’s required
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } // Remove the "= "User"" default value
    }
}