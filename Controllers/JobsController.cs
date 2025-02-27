using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobPortalContext _context;

        public JobsController(JobPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return await _context.Jobs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return job;
        }

        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob([FromBody] Job job)
        {
            // ✅ Debugging: Print the provided Company ID
            Console.WriteLine($"🔍 Checking Company ID: {job.CompanyId}");

            // ✅ Check if the Company exists
            var companyExists = await _context.Companies.AnyAsync(c => c.Id == job.CompanyId);
            if (!companyExists)
            {
                Console.WriteLine($"❌ Company ID {job.CompanyId} does not exist.");
                return BadRequest(new { message = "Company does not exist!" });
            }

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Job '{job.Title}' created successfully!");
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
