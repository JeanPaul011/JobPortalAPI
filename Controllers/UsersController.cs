using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobPortalAPI.Services;
using JobPortalAPI.DTOs;

namespace JobPortalAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ GET ALL USERS (Admin Only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ GET A SINGLE USER
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // ✅ UPDATE USER ROLE (Admin Only)
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleModel model)
        {
            bool success = await _userService.UpdateUserRoleAsync(id, model.Role);

            if (!success)
                return BadRequest("Failed to update user role. Check if the role exists.");

            return Ok(new { message = "User role updated successfully!", Role = model.Role });
        }


        // DTO for Updating User Role
        public class UpdateRoleModel
        {
            public required string Role { get; set; }
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

