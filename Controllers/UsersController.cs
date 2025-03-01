using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobPortalAPI.Services;
using JobPortalAPI.DTOs;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ✅ GET ALL USERS (Admin Only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users...");
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ✅ GET A SINGLE USER
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            try
            {
                _logger.LogInformation($"Fetching user with ID {id}");
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching user {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ✅ UPDATE USER ROLE (Admin Only)
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting to update role for user {id} to {model.Role}");
                bool success = await _userService.UpdateUserRoleAsync(id, model.Role);

                if (!success)
                {
                    _logger.LogWarning($"Role update failed for user {id}. Role may not exist.");
                    return BadRequest("Failed to update user role. Check if the role exists.");
                }

                _logger.LogInformation($"User role updated successfully for {id}");
                return Ok(new { message = "User role updated successfully!", Role = model.Role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user role for {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}



// [HttpGet]
// [Authorize(Roles = "Admin")]
// public async Task<ActionResult<IEnumerable<User>>> GetUsers()
// {
//     var loggedInUser = await _userManager.GetUserAsync(User);

//     // ✅ Debugging Logs
//     Console.WriteLine($"🔍 Checking Access: {loggedInUser?.Email}, Role: {loggedInUser?.Role}");

//     if (loggedInUser == null || loggedInUser.Role != "Admin")
//     {
//         Console.WriteLine("❌ Unauthorized Access Attempt!");
//         return Unauthorized(new { message = "Access Denied. Admins Only!" });
//     }

//     var users = await _context.Users.ToListAsync();
//     return Ok(users);
// }

//Made a comment today 27/02 at 20:58
// [HttpGet]
// [Authorize(Roles = "Admin")]
// public async Task<ActionResult<IEnumerable<User>>> GetUsers()
// {
//     var adminUser = await _userManager.GetUserAsync(User);

//     // 🔹 Extra Debugging Log
//     Console.WriteLine($"🔍 Checking Admin Access: {adminUser?.Email}, Role: {adminUser?.Role}");

//     // 🔹 If user is not found or not an Admin, return Unauthorized
//     if (adminUser == null || adminUser.Role != "Admin")
//     {
//         Console.WriteLine("❌ Unauthorized access: User is not an Admin.");
//         return Unauthorized(new { message = "Access Denied. Admins Only!" });
//     }

//     var users = await _context.Users.ToListAsync();
//     return Ok(users);
// }

// [HttpGet]
// //[Authorize(Roles = "Admin")]
// public async Task<ActionResult<IEnumerable<User>>> GetUsers()
// {

//     var users = await _context.Users.ToListAsync();
//     foreach (var user in users)
//     {
//         Console.WriteLine($"User: {user.Email}, Role: {user.Role}");  //  Debugging log
//     }

//     return Ok(users);
// }

