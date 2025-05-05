using JobPortalAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Repositories
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
{
    Task<IEnumerable<JobApplication>> GetApplicationsByJobIdAsync(int jobId);
    Task<IEnumerable<JobApplication>> GetApplicationsByUserIdAsync(string userId);
    Task<bool> UpdateApplicationStatusAsync(int id, string status);
    Task<bool> JobExistsAsync(int jobId);
    Task<bool> HasAppliedAsync(int jobId, string jobSeekerId);
}

}