using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobPortalAPI.DTOs;
using JobPortalAPI.Models;

namespace JobPortalAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Fetch roles dynamically
                userDTOs.Add(new UserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    Role = roles.Any() ? string.Join(", ", roles) : "No Role" // Role retrieval
                });
            }

            return userDTOs;
        }

        public async Task<UserDTO?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user); // Fetch roles dynamically
            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Role = roles.Any() ? string.Join(", ", roles) : "No Role" // Role retrieval
            };
        }

        // Update User Role Correctly
        public async Task<bool> UpdateUserRoleAsync(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            // Check if the role exists
            if (!await _roleManager.RoleExistsAsync(newRole))
                return false;

            // Remove existing roles before assigning a new one
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign the new role
            var result = await _userManager.AddToRoleAsync(user, newRole);
            if (!result.Succeeded)
                return false;

            return true;
        }
    }

    public class ServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
