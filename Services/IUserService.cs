using System.Collections.Generic;
using System.Threading.Tasks;
using JobPortalAPI.DTOs;

namespace JobPortalAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync(); // Fetch all users with roles
        Task<UserDTO?> GetUserByIdAsync(string id); //  Get single user with role
        Task<bool> UpdateUserRoleAsync(string id, string newRole);  // Update role properly
    }
}
