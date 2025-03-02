using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JobPortalAPI.Services;
using System.Threading.Tasks;
using JobPortalAPI.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace JobPortalAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
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

                // ✅ Generate Refresh Token
                user.RefreshToken = GenerateRefreshToken();
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpireDays"]));
                await _userManager.UpdateAsync(user);

                // ✅ Send Welcome Email
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

        // ✅ LOGIN & GET JWT + REFRESH TOKEN
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
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpireDays"]));

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("❌ Failed to save refresh token for {Email}", model.Email);
                    return StatusCode(500, "Failed to store refresh token.");
                }

                _logger.LogInformation($"✅ JWT Token & Refresh Token issued for {user.Email}");

                return Ok(new { Token = token, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in Login method");
                return StatusCode(500, "Internal server error");
            }
        }

        // ✅ REFRESH TOKEN
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == model.RefreshToken);

                if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning("❌ Invalid or expired refresh token.");
                    return Unauthorized("Invalid or expired refresh token.");
                }

                var newAccessToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpireDays"]));

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("❌ Failed to update refresh token for {Email}", user.Email);
                    return StatusCode(500, "Failed to update refresh token.");
                }

                _logger.LogInformation($"✅ New JWT & Refresh Token issued for {user.Email}");

                return Ok(new { Token = newAccessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error refreshing token.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ✅ JWT TOKEN GENERATION
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("role", user.Role ?? "JobSeeker")  
            };

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is missing in configuration."));
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"] ?? "1"));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            ));
        }

        // ✅ REFRESH TOKEN GENERATION
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }

    // ✅ REQUEST MODELS
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

    public class RefreshTokenModel
    {
        public required string RefreshToken { get; set; }
    }
}
