using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Models;

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]  // ✅ This ensures the correct API route
    [ApiController]
    [Authorize(Roles = "Admin")]// ✅ Only Admins Can Manage Roles
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            Console.WriteLine("✅ RolesController Loaded!");
        }

        // ✅ GET ALL ROLES
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        // ✅ GET A SINGLE ROLE
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound("Role not found.");
            return Ok(role);
        }

        // ✅ CREATE A NEW ROLE
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Role already exists.");

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
                return Ok("Role created successfully.");

            return BadRequest(result.Errors);
        }

        // ✅ DELETE A ROLE
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound("Role not found.");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return Ok("Role deleted successfully.");

            return BadRequest(result.Errors);
        }

        // ✅ ASSIGN A ROLE TO A USER
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
                return NotFound("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (result.Succeeded)
                return Ok("Role assigned to user successfully.");

            return BadRequest(result.Errors);
        }
    }

    // ✅ REQUEST MODEL FOR ASSIGNING ROLES
    public class AssignRoleModel
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
