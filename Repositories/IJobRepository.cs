using JobPortalAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Repositories
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<IEnumerable<Job>> GetJobsByCompanyIdAsync(int companyId);
        Task<bool> CompanyExistsAsync(int companyId);
        Task<IEnumerable<Job>> SearchJobsAsync(string title = null, string location = null, decimal? minSalary = null, decimal? maxSalary = null, string jobType = null);
        Task<Company> GetCompanyByNameAsync(string companyName);
        Task AddCompanyAsync(Company company);
        Task<IEnumerable<Job>> GetJobsByRecruiterIdAsync(string recruiterId);
        
    



    }
}