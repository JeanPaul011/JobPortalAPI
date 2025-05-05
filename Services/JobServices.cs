using JobPortalAPI.Models;
using JobPortalAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public class JobService : IJobService
    {
        private readonly JobPortalContext _context;

        public JobService(JobPortalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobDTO>> GetAllJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Company) // ✅ Include related company
                .Select(j => new JobDTO
                {
                    Id = j.Id,
                    Title = j.Title,
                    JobType = j.JobType,
                    Salary = j.Salary,
                    Location = j.Location, // ✅ Make sure to include Location
                    Description = j.Description, // ✅ Include Description
                    CompanyName = j.Company.Name // ✅ Use CompanyName instead of CompanyId
                }).ToListAsync();
        }

        public async Task<JobDTO?> GetJobByIdAsync(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Company) // ✅ Include related company
                .FirstOrDefaultAsync(j => j.Id == id);

            return job == null ? null : new JobDTO
            {
                Id = job.Id,
                Title = job.Title,
                JobType = job.JobType,
                Salary = job.Salary,
                Location = job.Location,
                Description = job.Description,
                CompanyName = job.Company.Name // ✅ Use CompanyName
            };
        }

        public async Task AddJobAsync(JobDTO jobDto)
        {
            // ✅ Check if the company exists by name
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Name.ToLower() == jobDto.CompanyName.ToLower());

            // ✅ If the company does not exist, create it
            if (company == null)
            {
                company = new Company { Name = jobDto.CompanyName };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }

            // ✅ Create the new job
            var job = new Job
            {
                Title = jobDto.Title,
                JobType = jobDto.JobType,
                Salary = jobDto.Salary,
                Location = jobDto.Location,
                Description = jobDto.Description,
                CompanyId = company.Id,
                RecruiterId = "System" // Optional: replace with actual recruiter ID if available
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteJobAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
            }
        }
    }
}

