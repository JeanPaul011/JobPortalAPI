using JobPortalAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
        Task<ReviewDTO?> GetReviewByIdAsync(int id);
        Task AddReviewAsync(ReviewDTO review);
        Task DeleteReviewAsync(int id);
    }
}
