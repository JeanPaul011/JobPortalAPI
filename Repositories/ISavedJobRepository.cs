using JobPortalAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortalAPI.Repositories
{
    public interface ISavedJobRepository
    {
        Task<IEnumerable<SavedJob>> GetSavedJobsByJobSeekerIdAsync(string jobSeekerId);
        Task AddAsync(SavedJob savedJob);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<SavedJob, bool>> predicate);
    }
}
