using JobPortalAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync();
        Task<CompanyDTO?> GetCompanyByIdAsync(int id);
        Task AddCompanyAsync(CompanyDTO company);
        Task DeleteCompanyAsync(int id);
    }
}
