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
        public async Task<Company> GetCompanyByNameAsync(string companyName)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Name.ToLower() == companyName.ToLower());
        }

        public async Task AddCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
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

        public async Task<IEnumerable<Job>> GetJobsByRecruiterIdAsync(string recruiterId)
        {
            return await _context.Jobs
                .Where(j => j.RecruiterId == recruiterId)
                .ToListAsync();
        }

    }
}