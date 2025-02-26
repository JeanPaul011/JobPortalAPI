using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly JobPortalContext _context;

        public JobApplicationsController(JobPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications()
        {
            return await _context.JobApplications.Include(j => j.Job).Include(j => j.JobSeeker).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplication>> GetJobApplication(int id)
        {
            var jobApplication = await _context.JobApplications
                .Include(j => j.Job)
                .Include(j => j.JobSeeker)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobApplication == null)
            {
                return NotFound();
            }
            return jobApplication;
        }

        [HttpPost]
        [Authorize(Roles = "JobSeeker")]
        public async Task<ActionResult<JobApplication>> ApplyForJob(JobApplication jobApplication)
        {
            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetJobApplication), new { id = jobApplication.Id }, jobApplication);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            _context.JobApplications.Remove(jobApplication);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
