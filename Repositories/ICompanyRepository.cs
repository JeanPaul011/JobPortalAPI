using JobPortalAPI.Models;

namespace JobPortalAPI.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<IEnumerable<Company>> GetCompaniesWithJobsAsync();
    }
}
