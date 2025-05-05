// // TokenController.cs
// using System.Security.Claims;
// using JobPortalAPI.Models;
// using JobPortalAPI.Services.TokenServices;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace JobPortalAPI.Controllers
// {
//     [Route("api/account")]
//     [ApiController]
//     public class TokenController : ControllerBase
//     {
//         private readonly ITokenService _tokenService;
//         private readonly UserManager<User> _userManager;

//         public TokenController(ITokenService tokenService, UserManager<User> userManager)
//         {
//             _tokenService = tokenService;
//             _userManager = userManager;
//         }

//         [HttpPost("refresh-token")]
//         public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
//         {
//             if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
//                 return BadRequest("Invalid client request");

//             var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
//             var userId = principal.FindFirst("userId")?.Value;

//             if (userId == null)
//                 return BadRequest("Invalid token");

//             var user = await _userManager.FindByIdAsync(userId);
//             if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
//                 return BadRequest("Invalid refresh token");

//             var newAccessToken = _tokenService.GenerateToken(user);
//             var newRefreshToken = _tokenService.GenerateRefreshToken();

//             user.RefreshToken = newRefreshToken;
//             user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
//             await _userManager.UpdateAsync(user);

//             return Ok(new TokenResponse
//             {
//                 Token = newAccessToken,
//                 RefreshToken = newRefreshToken
//             });
//         }
//     }

//     public class RefreshTokenRequest
//     {
//         public string AccessToken { get; set; }
//         public string RefreshToken { get; set; }
//     }

//     public class TokenResponse
//     {
//         public string Token { get; set; }
//         public string RefreshToken { get; set; }
//     }
// }