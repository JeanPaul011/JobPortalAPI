using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalAPI.Repositories
{
    public class SavedJobRepository : ISavedJobRepository
    {
        private readonly JobPortalContext _context;

        public SavedJobRepository(JobPortalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SavedJob>> GetSavedJobsByJobSeekerIdAsync(string jobSeekerId)
        {
            return await _context.SavedJobs
                .Where(sj => sj.JobSeekerId == jobSeekerId)
                .ToListAsync();
        }

        public async Task AddAsync(SavedJob savedJob)
        {
            await _context.SavedJobs.AddAsync(savedJob);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var savedJob = await _context.SavedJobs.FindAsync(id);
            if (savedJob != null)
            {
                _context.SavedJobs.Remove(savedJob);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<SavedJob, bool>> predicate)
        {
            return await _context.SavedJobs.AnyAsync(predicate);
        }
    }
}
