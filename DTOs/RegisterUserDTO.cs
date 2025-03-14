using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.DTOs
{
    public class RegisterUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string? FullName { get; set; }
    }
}
