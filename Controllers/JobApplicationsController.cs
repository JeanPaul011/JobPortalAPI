using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using JobPortalAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using JobPortalAPI.DTOs;

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
        // Only JobSeekers can apply for jobs
[HttpPost]
[Authorize(Roles = "JobSeeker")]
public async Task<ActionResult<JobApplication>> ApplyForJob([FromBody] JobApplicationCreateDTO dto)
{
    var jobExists = await _jobApplicationRepository.JobExistsAsync(dto.JobId);
    if (!jobExists)
        return NotFound($"Job with ID {dto.JobId} does not exist.");

    var jobSeekerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(jobSeekerId))
        return Unauthorized("Job seeker identity could not be verified.");

    var alreadyApplied = await _jobApplicationRepository.HasAppliedAsync(dto.JobId, jobSeekerId);
    if (alreadyApplied)
        return Conflict("You have already applied to this job.");

    var application = new JobApplication
    {
        JobId = dto.JobId,
        JobSeekerId = jobSeekerId,
        Message = dto.Message,
        Status = "Pending",
        AppliedOn = DateTime.UtcNow
    };

    await _jobApplicationRepository.AddAsync(application);
    return CreatedAtAction(nameof(GetJobApplications), new { id = application.Id }, application);
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
