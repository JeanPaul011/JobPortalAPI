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
                .Select(j => new JobDTO
                {
                    Id = j.Id,
                    Title = j.Title,
                    JobType = j.JobType,
                    Salary = j.Salary,
                    CompanyId = j.CompanyId
                }).ToListAsync();
        }

        public async Task<JobDTO?> GetJobByIdAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            return job == null ? null : new JobDTO
            {
                Id = job.Id,
                Title = job.Title,
                JobType = job.JobType,
                Salary = job.Salary,
                CompanyId = job.CompanyId
            };
        }

        public async Task AddJobAsync(JobDTO jobDto)
        {
            var job = new Job
            {
                Title = jobDto.Title,
                JobType = jobDto.JobType,
                Salary = jobDto.Salary,
                CompanyId = jobDto.CompanyId
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
