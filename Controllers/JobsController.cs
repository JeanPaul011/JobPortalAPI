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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return Ok(await _jobRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost]
        //[Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<Job>> CreateJob([FromBody] Job job)
        {
            var companyExists = await _jobRepository.CompanyExistsAsync(job.CompanyId);
            if (!companyExists)
                return BadRequest("Company does not exist!");

            await _jobRepository.AddAsync(job);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Recruiter, Admin")]
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
