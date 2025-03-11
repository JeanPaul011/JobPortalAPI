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

        // Only Admins & Recruiters can view applications
        [HttpGet]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications()
        {
            return Ok(await _jobApplicationRepository.GetAllAsync());
        }

        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetApplicationsByJob(int jobId)
        {
            _logger.LogInformation($"Fetching applications for job with ID {jobId}");

            try
            {
                var applications = await _jobApplicationRepository.GetApplicationsByJobIdAsync(jobId);
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching applications for job with ID {jobId}");
                return StatusCode(500, "An error occurred while fetching job applications");
            }
        }

        // Only JobSeekers can apply for jobs
        [HttpPost]
        [Authorize(Roles = "JobSeeker")]
        public async Task<ActionResult<JobApplication>> ApplyForJob(JobApplication jobApplication)
        {
            await _jobApplicationRepository.AddAsync(jobApplication);
            return CreatedAtAction(nameof(GetJobApplications), new { id = jobApplication.Id }, jobApplication);
        }
        // PUT: api/jobapplications/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] string status)
        {
            _logger.LogInformation($"Updating status for job application with ID {id}");

            var application = await _jobApplicationRepository.GetByIdAsync(id);
            if (application == null)
            {
                _logger.LogWarning($"Job application with ID {id} not found");
                return NotFound();
            }

            try
            {
                // Validate status
                if (!new[] { "Pending", "Reviewing", "Accepted", "Rejected", "Withdrawn" }.Contains(status))
                {
                    _logger.LogWarning($"Invalid status value: {status}");
                    return BadRequest("Invalid application status. Valid values are: Pending, Reviewing, Accepted, Rejected, Withdrawn");
                }

                application.Status = status;
                await _jobApplicationRepository.UpdateAsync(application);
                _logger.LogInformation($"Job application with ID {id} status updated to {status}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for job application with ID {id}");
                return StatusCode(500, "An error occurred while updating the application status");
            }
        }

        // Only Admins & Recruiters can delete job applications
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recruiter")]
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
