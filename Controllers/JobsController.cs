using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobPortalContext _context;
        private readonly ILogger<JobsController> _logger;

        public JobsController(JobPortalContext context, ILogger<JobsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            try
            {
                _logger.LogInformation("Fetching all jobs...");
                return await _context.Jobs.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching job with ID {id}");
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    _logger.LogWarning($"Job with ID {id} not found.");
                    return NotFound();
                }
                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving job {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob([FromBody] Job job)
        {
            try
            {
                _logger.LogInformation($"Creating job: {job.Title}");

                var companyExists = await _context.Companies.AnyAsync(c => c.Id == job.CompanyId);
                if (!companyExists)
                {
                    _logger.LogWarning($"Company ID {job.CompanyId} does not exist.");
                    return BadRequest("Company does not exist!");
                }

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Job '{job.Title}' created successfully!");
                return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    _logger.LogWarning($"Job with ID {id} not found.");
                    return NotFound();
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Job with ID {id} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting job {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
