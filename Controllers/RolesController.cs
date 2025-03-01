using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Models;
using Microsoft.Extensions.Logging; // Added Logging

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Only Admins Can Manage Roles
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RolesController> _logger; // Added Logging

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            try
            {
                _logger.LogInformation("Fetching all roles.");
                var roles = _roleManager.Roles;
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roles.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role '{Role}'", roleName);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
