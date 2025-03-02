using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortalAPI.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(JobPortalContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetReviewsByCompanyIdAsync(int companyId)
        {
            return await _context.Reviews
                .Where(r => r.CompanyId == companyId)
                .ToListAsync();
        }
    }
}
