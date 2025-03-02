using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Models;
using JobPortalAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
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

