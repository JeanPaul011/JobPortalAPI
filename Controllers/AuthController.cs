using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobPortalAPI.Services;
using System.Threading.Tasks;
using JobPortalAPI.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging; // Added Logging

namespace JobPortalAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger; // Added Logging

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger) // Inject Logger
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        // ✅ REGISTER USER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthModel model, [FromServices] EmailService emailService)
        {
            try
            {
                _logger.LogInformation("🔹 Registering user: {Email}", model.Email);

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("❌ User already exists: {Email}", model.Email);
                    return BadRequest(new { message = "User already exists!" });
                }

                if (model.Role != "Admin" && model.Role != "Recruiter" && model.Role != "JobSeeker")
                {
                    _logger.LogWarning("❌ Invalid role: {Role}", model.Role);
                    return BadRequest(new { message = "Invalid role. Allowed: Admin, Recruiter, JobSeeker." });
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = model.Role
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("❌ User creation failed: {Errors}", result.Errors);
                    return BadRequest(result.Errors);
                }

                await _userManager.AddToRoleAsync(user, model.Role);

                // Send Welcome Email
                string subject = "Welcome to Job Portal!";
                string body = $"<h3>Hi {user.FullName},</h3><p>Your account has been successfully created.</p>";
                await emailService.SendEmailAsync(user.Email, subject, body);

                _logger.LogInformation("✅ User registered successfully: {Email}", user.Email);
                return Ok(new { message = "User registered successfully! An email has been sent.", user.Role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in Register method");
                return StatusCode(500, "Internal server error");
            }
        }

        // LOGIN & GET JWT TOKEN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                _logger.LogInformation("🔹 Login attempt: {Email}", model.Email);

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("❌ Invalid login attempt: {Email}", model.Email);
                    return Unauthorized("Invalid credentials.");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("❌ Invalid login credentials: {Email}", model.Email);
                    return Unauthorized("Invalid credentials.");
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("JWT Token Created for {Email}", user.Email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in Login method");
                return StatusCode(500, "Internal server error");
            }
        }

        // GENERATE JWT TOKEN
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("role", user.Role ?? "JobSeeker")  // ✅ Change this line

            };
             Console.WriteLine($"🔹 Claims in JWT: {string.Join(", ", claims.Select(c => c.Type + ": " + c.Value))}");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"] ?? "1"));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            ));
        }
    }

    // ✅ LOGIN / REGISTER MODELS
    public class AuthModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
    }

    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
