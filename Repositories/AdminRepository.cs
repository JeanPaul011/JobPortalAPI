// Repositories/AdminRequestRepository.cs
// ✅ this is where ApplicationDbContext is
using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
 // ✅ this is where ApplicationDbContext is


public class AdminRequestRepository : IAdminRequestRepository
{
    private readonly JobPortalContext
 _context;

    public AdminRequestRepository(JobPortalContext context)

    {
        _context = context;
    }

    public async Task<IEnumerable<AdminRequest>> GetPendingRequestsAsync()
    {
        return await _context.AdminRequests
            .Where(r => r.Status == "Pending")
            .ToListAsync();
    }

    public async Task<AdminRequest?> GetRequestByIdAsync(int id)
    {
        return await _context.AdminRequests.FindAsync(id);
    }

    public async Task AddRequestAsync(AdminRequest request)
    {
        _context.AdminRequests.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task ApproveRequestAsync(AdminRequest request)
    {
        request.Status = "Approved";
        await _context.SaveChangesAsync();
    }

    public async Task RejectRequestAsync(AdminRequest request)
    {
        request.Status = "Rejected";
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRequestAsync(AdminRequest request)
{
    _context.AdminRequests.Update(request);
    await _context.SaveChangesAsync();
}

}
