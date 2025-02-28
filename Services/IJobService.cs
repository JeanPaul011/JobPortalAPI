using JobPortalAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public interface IJobService
    {
        Task<IEnumerable<JobDTO>> GetAllJobsAsync();
        Task<JobDTO?> GetJobByIdAsync(int id);
        Task AddJobAsync(JobDTO job);
        Task DeleteJobAsync(int id);
    }
}
