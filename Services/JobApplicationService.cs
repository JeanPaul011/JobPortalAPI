using JobPortalAPI.Models;
using JobPortalAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly JobPortalContext _context;

        public JobApplicationService(JobPortalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobApplicationDTO>> GetAllJobApplicationsAsync()
        {
            return await _context.JobApplications
                .Select(ja => new JobApplicationDTO
                {
                    Id = ja.Id,
                    JobId = ja.JobId,
                    UserId = ja.JobSeekerId,
                    Status = ja.Status,
                    Message = ja.Message
                }).ToListAsync();
        }

        public async Task<JobApplicationDTO?> GetJobApplicationByIdAsync(int id)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);
            return jobApplication == null ? null : new JobApplicationDTO
            {
                Id = jobApplication.Id,
                JobId = jobApplication.JobId,
                UserId = jobApplication.JobSeekerId,
                Status = jobApplication.Status,
                Message = jobApplication.Message
            };
        }

        public async Task AddJobApplicationAsync(JobApplicationDTO dto)
        {
            var jobApplication = new JobApplication
            {
                JobId = dto.JobId,
                JobSeekerId = dto.UserId,
                Status = dto.Status,
                Message = dto.Message,
                AppliedOn = DateTime.UtcNow
            };

            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateJobApplicationStatusAsync(int id, string status)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication != null)
            {
                jobApplication.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteJobApplicationAsync(int id)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication != null)
            {
                _context.JobApplications.Remove(jobApplication);
                await _context.SaveChangesAsync();
            }
        }
    }
}