using JobPortalAPI.Models;
using JobPortalAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly JobPortalContext _context;

        public ReviewService(JobPortalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    CompanyId = r.CompanyId,
                    Content = r.Comment,  // ✅ FIXED: Changed Content → Comment
                    Rating = r.Rating
                }).ToListAsync();
        }

        public async Task<ReviewDTO?> GetReviewByIdAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            return review == null ? null : new ReviewDTO
            {
                Id = review.Id,
                CompanyId = review.CompanyId,
                Content = review.Comment,  // ✅ FIXED: Changed Content → Comment
                Rating = review.Rating
            };
        }

        public async Task AddReviewAsync(ReviewDTO reviewDto)
        {
            var review = new Review
            {
                CompanyId = reviewDto.CompanyId,
                Comment = reviewDto.Content,  // ✅ FIXED: Changed Content → Comment
                Rating = reviewDto.Rating
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}
