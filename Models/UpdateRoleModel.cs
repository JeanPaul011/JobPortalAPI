using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.Models.Roles
{
    public class UpdateRoleModel
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public string NewRoleName { get; set; } = string.Empty;
    }
}
