using JobPortalAPI.Models;

namespace JobPortalAPI.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByCompanyIdAsync(int companyId);
    }
}
