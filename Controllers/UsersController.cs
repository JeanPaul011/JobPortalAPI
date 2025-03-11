using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using JobPortalAPI.Models;
using JobPortalAPI.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using JobPortalAPI.DTOs;

namespace JobPortalAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager; // Inject UserManager
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, UserManager<User> userManager, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager; // Store UserManager reference
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] // Only Admins can access
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Fetch correct roles from Identity
                userDTOs.Add(new UserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Role = roles.Any() ? string.Join(", ", roles) : "No Role", // Get actual assigned roles
                    Email = user.Email ?? ""
                });
            }

            return Ok(userDTOs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can access
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user); // Fetch roles dynamically
            var userDTO = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = roles.Any() ? string.Join(", ", roles) : "No Role",
                Email = user.Email ?? ""
            };

            return Ok(userDTO);
        }
    }
}
