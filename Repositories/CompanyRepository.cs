using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortalAPI.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(JobPortalContext context) : base(context) { }

        public async Task<IEnumerable<Company>> GetCompaniesWithJobsAsync()
        {
            return await _context.Companies
                .Include(c => c.Jobs)
                .ToListAsync();
        }
    }
}
