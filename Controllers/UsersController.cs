using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobPortalAPI.Models;

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JobPortalContext _context;

        public UsersController(UserManager<User> userManager, JobPortalContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // ✅ GET ALL USERS (Admin Only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);

            // 🔥 Debugging: Print What ASP.NET Sees
            Console.WriteLine("🔍 User Claims:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            Console.WriteLine($"🔍 Checking Access: {loggedInUser?.Email}, Role: {loggedInUser?.Role}");

            if (loggedInUser == null || loggedInUser.Role != "Admin")
            {
                Console.WriteLine("❌ Unauthorized Access Attempt!");
                return Unauthorized(new { message = "Access Denied. Admins Only!" });
            }

            var users = await _context.Users.ToListAsync();
            return Ok(users);
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

        //GET A SINGLE USER
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // ✅ UPDATE USER ROLE (Admin Only)
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            // ✅ Validate Role (Only "Admin", "Recruiter", "JobSeeker" allowed)
            if (model.Role != "Admin" && model.Role != "Recruiter" && model.Role != "JobSeeker")
                return BadRequest("Invalid role. Allowed: Admin, Recruiter, JobSeeker.");

            user.Role = model.Role;
            await _userManager.UpdateAsync(user);
            return Ok(new { message = "User role updated successfully!", user.Role });
        }
    }

    public class UpdateRoleModel
    {
        public required string Role { get; set; }
    }
}
