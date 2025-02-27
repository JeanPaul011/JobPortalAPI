using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobPortalAPI.Services;  // ✅ Make sure this is added
using System.Threading.Tasks;
using JobPortalAPI.Models;
using System;
using System.Collections.Generic;

namespace JobPortalAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // ✅ REGISTER USER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthModel model, [FromServices] EmailService emailService)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User already exists!" });

            if (model.Role != "Admin" && model.Role != "Recruiter" && model.Role != "JobSeeker")
                return BadRequest(new { message = "Invalid role. Allowed: Admin, Recruiter, JobSeeker." });

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, model.Role);

            // ✅ Send Welcome Email
            string subject = "Welcome to Job Portal!";
            string body = $"<h3>Hi {user.FullName},</h3><p>Your account has been successfully created.</p>";
            await emailService.SendEmailAsync(user.Email, subject, body);

            Console.WriteLine($"📧 Sent welcome email to {user.Email}");

            return Ok(new { message = "User registered successfully! An email has been sent.", user.Role });
        }

        // ✅ LOGIN & GET JWT TOKEN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials.");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        // ✅ GENERATE JWT TOKEN
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "JobSeeker")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"] ?? "1"));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            Console.WriteLine($"🔐 JWT Token Created for {user.Email} | Role: {user.Role}");

            return new JwtSecurityTokenHandler().WriteToken(token);
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
