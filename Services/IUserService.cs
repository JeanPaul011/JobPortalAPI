using System.Collections.Generic;
using System.Threading.Tasks;
using JobPortalAPI.DTOs;

namespace JobPortalAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserRoleAsync(string id, string newRole);  // ✅ Add this line
    }
}
