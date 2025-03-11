using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalAPI.Repositories
{
    public class JobApplicationRepository : Repository<JobApplication>, IJobApplicationRepository
    {
        public JobApplicationRepository(JobPortalContext context) : base(context) { }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByJobIdAsync(int jobId)
        {
            return await _context.JobApplications
                .Where(j => j.JobId == jobId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<JobApplication>> GetApplicationsByUserIdAsync(string userId)
        {
            return await _context.JobApplications
                .Where(a => a.JobSeekerId == userId)
                .ToListAsync();
        }
        
        public async Task<bool> UpdateApplicationStatusAsync(int id, string status)
        {
            var application = await _context.JobApplications.FindAsync(id);
            
            if (application == null)
                return false;
                
            application.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}