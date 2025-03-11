using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalAPI.Repositories
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        public JobRepository(JobPortalContext context) : base(context) { }

        public async Task<IEnumerable<Job>> GetJobsByCompanyIdAsync(int companyId)
        {
            return await _context.Jobs
                .Where(j => j.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<bool> CompanyExistsAsync(int companyId)
        {
            return await _context.Companies.AnyAsync(c => c.Id == companyId);
        }
        
        public async Task<IEnumerable<Job>> SearchJobsAsync(string title = null, string location = null, decimal? minSalary = null, decimal? maxSalary = null, string jobType = null)
        {
            var query = _context.Jobs.AsQueryable();
            
            if (!string.IsNullOrEmpty(title))
                query = query.Where(j => j.Title.Contains(title));
            
            if (!string.IsNullOrEmpty(location))
                query = query.Where(j => j.Location.Contains(location));
            
            if (minSalary.HasValue)
                query = query.Where(j => j.Salary >= minSalary.Value);
            
            if (maxSalary.HasValue)
                query = query.Where(j => j.Salary <= maxSalary.Value);
            
            if (!string.IsNullOrEmpty(jobType))
                query = query.Where(j => j.JobType == jobType);
            
            return await query.ToListAsync();
        }
    }
}