using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; // Added Logging

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly JobPortalContext _context;
        private readonly ILogger<JobApplicationsController> _logger; // Added Logging

        public JobApplicationsController(JobPortalContext context, ILogger<JobApplicationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications()
        {
            try
            {
                _logger.LogInformation("Fetching all job applications.");
                return await _context.JobApplications.Include(j => j.Job).Include(j => j.JobSeeker).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job applications.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "JobSeeker")]
        public async Task<ActionResult<JobApplication>> ApplyForJob(JobApplication jobApplication)
        {
            try
            {
                _logger.LogInformation("Applying for job with ID {JobId} by user {JobSeekerId}", jobApplication.JobId, jobApplication.JobSeekerId);
                _context.JobApplications.Add(jobApplication);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetJobApplications), new { id = jobApplication.Id }, jobApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying for job.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            try
            {
                _logger.LogInformation("Deleting job application with ID {Id}", id);
                var jobApplication = await _context.JobApplications.FindAsync(id);
                if (jobApplication == null)
                {
                    _logger.LogWarning("Job application with ID {Id} not found.", id);
                    return NotFound();
                }

                _context.JobApplications.Remove(jobApplication);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Job application with ID {Id} deleted.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job application with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
