using JobPortalAPI.Models;

namespace JobPortalAPI.Repositories
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
    {
        Task<IEnumerable<JobApplication>> GetApplicationsByJobIdAsync(int jobId);
    }
}
