// ITokenService.cs
using System.Security.Claims;
using JobPortalAPI.Models;

namespace JobPortalAPI.Services.TokenServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}