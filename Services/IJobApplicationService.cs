 using JobPortalAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public interface IJobApplicationService
    {
        Task<IEnumerable<JobApplicationDTO>> GetAllJobApplicationsAsync();
        Task<JobApplicationDTO?> GetJobApplicationByIdAsync(int id);
        Task AddJobApplicationAsync(JobApplicationDTO jobApplication);
        Task UpdateJobApplicationStatusAsync(int id, string status);
        Task DeleteJobApplicationAsync(int id);
    }
} 