using JobPortalAPI.Models;

namespace JobPortalAPI.Repositories
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<IEnumerable<Job>> GetJobsByCompanyIdAsync(int companyId);
        Task<bool> CompanyExistsAsync(int companyId); // ✅ Add this method
    }
}
