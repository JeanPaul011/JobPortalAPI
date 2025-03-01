using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; // ✅ Added Logging

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly JobPortalContext _context;
        private readonly ILogger<ReviewsController> _logger; // ✅ Added Logging

        public ReviewsController(JobPortalContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            try
            {
                _logger.LogInformation("Fetching all reviews.");
                return await _context.Reviews.Include(r => r.Company).Include(r => r.User).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            try
            {
                _logger.LogInformation("Fetching review with ID {Id}", id);
                var review = await _context.Reviews.Include(r => r.Company).Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (review == null)
                {
                    _logger.LogWarning("Review with ID {Id} not found.", id);
                    return NotFound();
                }
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching review with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "JobSeeker")]
        public async Task<ActionResult<Review>> CreateReview(Review review)
        {
            try
            {
                _logger.LogInformation("Creating a new review for company ID {CompanyId}", review.CompanyId);
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                _logger.LogInformation("Deleting review with ID {Id}", id);
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review with ID {Id} not found.", id);
                    return NotFound();
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Review with ID {Id} deleted.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

