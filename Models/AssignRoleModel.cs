using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.Models.Roles
{
    public class AssignRoleModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
