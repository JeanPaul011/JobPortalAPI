using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Models;
using Microsoft.Extensions.Logging;
using JobPortalAPI.Models.Roles;

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        // ✅ Public: Get all roles
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetRoles()
        {
            _logger.LogInformation("Fetching all roles.");
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        // 🔒 Admin only: Get specific role
        [HttpGet("{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return NotFound("Role not found.");
            }
            return Ok(role);
        }

        // 🔒 Admin only: Create role
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning("Role '{Role}' already exists.", roleName);
                return BadRequest("Role already exists.");
            }

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role '{Role}' created successfully.", roleName);
                return Ok("Role created successfully.");
            }

            _logger.LogError("Error creating role '{Role}': {Errors}", roleName, result.Errors);
            return BadRequest(result.Errors);
        }

        // 🔒 Admin only: Get user roles
        [HttpGet("get-user-roles/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { userId = user.Id, roles });
        }

        // 🔒 Admin only: Update role
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", model.RoleId);
                return NotFound("Role not found.");
            }

            role.Name = model.NewRoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role '{RoleId}' updated to '{NewRoleName}'", model.RoleId, model.NewRoleName);
                return Ok("Role updated successfully.");
            }

            _logger.LogError("Error updating role '{RoleId}': {Errors}", model.RoleId, result.Errors);
            return BadRequest(result.Errors);
        }

        // 🔒 Admin only: Delete role
        [HttpDelete("{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role '{RoleId}' deleted successfully.", roleId);
                return Ok("Role deleted successfully.");
            }

            _logger.LogError("Error deleting role '{RoleId}': {Errors}", roleId, result.Errors);
            return BadRequest(result.Errors);
        }

        // 🔒 Admin only: Assign role to user
        [HttpPost("assign-role-to-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
            {
                return NotFound("Role not found.");
            }

            // Remove existing roles before assigning a new one
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign the new role
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Update user role property
            user.Role = model.RoleName;
            await _userManager.UpdateAsync(user);

            return Ok("Role assigned successfully.");
        }
    }
}
