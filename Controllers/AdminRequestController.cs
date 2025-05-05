// Controllers/AdminRequestController.cs
using JobPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

 // Ensure you have this for User


[Route("api/[controller]")]
[ApiController]
public class AdminRequestController : ControllerBase
{
    private readonly IAdminRequestRepository _repository;
    private readonly UserManager<User> _userManager;

    public AdminRequestController(IAdminRequestRepository repository, UserManager<User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    [HttpPost("request")]
    [Authorize] // User must be logged in to request admin
    public async Task<IActionResult> RequestAdmin()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

         var request = new AdminRequest { UserId = userId };
         await _repository.AddRequestAsync(request);

         return Ok(new { message = "Admin request submitted successfully!" });
     }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var requests = await _repository.GetPendingRequestsAsync();
        return Ok(requests);
    }

    [HttpPost("approve/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var request = await _repository.GetRequestByIdAsync(id);
        if (request == null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound();

        user.Role = "Admin";
        await _userManager.UpdateAsync(user);
        await _userManager.AddToRoleAsync(user, "Admin");

        await _repository.ApproveRequestAsync(request);

        return Ok(new { message = "Admin request approved!" });
    }

    [HttpPost("reject/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectRequest(int id)
    {
        var request = await _repository.GetRequestByIdAsync(id);
        if (request == null)
            return NotFound();

        await _repository.RejectRequestAsync(request);

        return Ok(new { message = "Admin request rejected!" });
    }
}


