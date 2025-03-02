using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using JobPortalAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/jobapplications")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ILogger<JobApplicationsController> _logger;

        public JobApplicationsController(IJobApplicationRepository jobApplicationRepository, ILogger<JobApplicationsController> logger)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications()
        {
            return Ok(await _jobApplicationRepository.GetAllAsync());
        }

        [HttpPost]
        //[Authorize(Roles = "JobSeeker")]
        public async Task<ActionResult<JobApplication>> ApplyForJob(JobApplication jobApplication)
        {
            await _jobApplicationRepository.AddAsync(jobApplication);
            return CreatedAtAction(nameof(GetJobApplications), new { id = jobApplication.Id }, jobApplication);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            var exists = await _jobApplicationRepository.ExistsAsync(j => j.Id == id);
            if (!exists)
                return NotFound();

            await _jobApplicationRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
