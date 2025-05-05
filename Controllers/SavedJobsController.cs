using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Models;
using JobPortalAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/savedjobs")]
    [ApiController]
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobRepository _savedJobRepository;
        private readonly ILogger<SavedJobsController> _logger;

        public SavedJobsController(ISavedJobRepository savedJobRepository, ILogger<SavedJobsController> logger)
        {
            _savedJobRepository = savedJobRepository;
            _logger = logger;
        }

        // GET: api/savedjobs/jobseeker/{jobSeekerId}
        [HttpGet("jobseeker/{jobSeekerId}")]
        [Authorize(Roles = "Jobseeker")]
        public async Task<ActionResult<IEnumerable<SavedJob>>> GetSavedJobs(string jobSeekerId)
        {
            _logger.LogInformation($"Fetching saved jobs for user {jobSeekerId}");

            try
            {
                var savedJobs = await _savedJobRepository.GetSavedJobsByJobSeekerIdAsync(jobSeekerId);
                return Ok(savedJobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching saved jobs for {jobSeekerId}");
                return StatusCode(500, "An error occurred while fetching saved jobs.");
            }
        }

        // POST: api/savedjobs
        [HttpPost]
        [Authorize(Roles = "Jobseeker")]
        public async Task<ActionResult<SavedJob>> SaveJob([FromBody] SavedJob savedJob)
        {
            _logger.LogInformation($"Saving job {savedJob.JobId} for user {savedJob.JobSeekerId}");

            try
            {
                await _savedJobRepository.AddAsync(savedJob);
                return CreatedAtAction(nameof(GetSavedJobs), new { jobSeekerId = savedJob.JobSeekerId }, savedJob);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving job");
                return StatusCode(500, "An error occurred while saving the job.");
            }
        }

        // DELETE: api/savedjobs/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Jobseeker")]
        public async Task<IActionResult> DeleteSavedJob(int id)
        {
            _logger.LogInformation($"Deleting saved job with ID {id}");

            var exists = await _savedJobRepository.ExistsAsync(sj => sj.Id == id);
            if (!exists)
                return NotFound();

            try
            {
                await _savedJobRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting saved job with ID {id}");
                return StatusCode(500, "An error occurred while deleting the saved job.");
            }
        }
    }
}
