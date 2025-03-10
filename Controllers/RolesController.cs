using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Models;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Admins only
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

        // Get all roles
        [HttpGet]
        public IActionResult GetRoles()
        {
            _logger.LogInformation("Fetching all roles.");
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        // Get a specific role by ID
        [HttpGet("{roleId}")]
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

        // Create a new role
        [HttpPost]
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
        [HttpGet("get-user-roles/{userId}")]
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


        // Update an existing role
        [HttpPut]
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

        // Delete a role
        [HttpDelete("{roleId}")]
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

        // Assign a role to a user
       [HttpPost("assign-role-to-user")]
public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
{
    var user = await _userManager.FindByIdAsync(model.UserId);
    if (user == null)
    {
        _logger.LogWarning("User not found: {UserId}", model.UserId);
        return NotFound("User not found.");
    }

    var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
    if (!roleExists)
    {
        _logger.LogWarning("Role not found: {RoleName}", model.RoleName);
        return NotFound("Role not found.");
    }

    // ✅ Remove all previous roles before assigning the new role
    var currentRoles = await _userManager.GetRolesAsync(user);
    await _userManager.RemoveFromRolesAsync(user, currentRoles);

    // ✅ Assign the new role
    var result = await _userManager.AddToRoleAsync(user, model.RoleName);
    if (result.Succeeded)
    {
        _logger.LogInformation("Role '{RoleName}' assigned to user '{UserId}' successfully.", model.RoleName, model.UserId);
        return Ok($"User {user.Email} is now assigned to role {model.RoleName}.");
    }

    _logger.LogError("Error assigning role '{RoleName}' to user '{UserId}': {Errors}", model.RoleName, model.UserId, result.Errors);
    return BadRequest(result.Errors);
}

    }
}
