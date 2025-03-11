using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Models;
using JobPortalAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobRepository jobRepository, ILogger<JobsController> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        // Everyone can view jobs
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return Ok(await _jobRepository.GetAllAsync());
        }

        // Everyone can view specific job details
        [HttpGet("{id}")]

        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound();

            return Ok(job);
        }

        // Only Admins & Recruiters can create jobs
        [HttpPost]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<ActionResult<Job>> CreateJob([FromBody] Job job)
        {
            var companyExists = await _jobRepository.CompanyExistsAsync(job.CompanyId);
            if (!companyExists)
                return BadRequest("Company does not exist!");

            await _jobRepository.AddAsync(job);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }

        // PUT: api/jobs/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] Job job)
        {
            _logger.LogInformation($"Updating job with ID {id}");

            if (id != job.Id)
            {
                _logger.LogWarning("Job ID mismatch");
                return BadRequest("ID in URL does not match ID in request body");
            }

            var exists = await _jobRepository.ExistsAsync(j => j.Id == id);
            if (!exists)
            {
                _logger.LogWarning($"Job with ID {id} not found for update");
                return NotFound();
            }

            try
            {
                await _jobRepository.UpdateAsync(job);
                _logger.LogInformation($"Job with ID {id} updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating job with ID {id}");
                return StatusCode(500, "An error occurred while updating the job");
            }
        }

        // GET: api/jobs/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Job>>> SearchJobs([FromQuery] string? title = null, [FromQuery] string? location = null, [FromQuery] decimal? minSalary = null, [FromQuery] decimal? maxSalary = null, [FromQuery] string? jobType = null)
        {
            _logger.LogInformation("Searching jobs with filters");

            try
            {
                var allJobs = await _jobRepository.GetAllAsync();
                var filteredJobs = allJobs.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(title))
                    filteredJobs = filteredJobs.Where(j => j.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(location))
                    filteredJobs = filteredJobs.Where(j => j.Location.Contains(location, StringComparison.OrdinalIgnoreCase));

                if (minSalary.HasValue)
                    filteredJobs = filteredJobs.Where(j => j.Salary >= minSalary.Value);

                if (maxSalary.HasValue)
                    filteredJobs = filteredJobs.Where(j => j.Salary <= maxSalary.Value);

                if (!string.IsNullOrEmpty(jobType))
                    filteredJobs = filteredJobs.Where(j => j.JobType.Equals(jobType, StringComparison.OrdinalIgnoreCase));

                return Ok(filteredJobs.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs with filters");
                return StatusCode(500, "An error occurred while searching for jobs");
            }
        }

        // Only Admins & Recruiters can delete jobs
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var exists = await _jobRepository.ExistsAsync(j => j.Id == id);
            if (!exists)
                return NotFound();

            await _jobRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
