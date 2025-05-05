// Repositories/IAdminRequestRepository.cs
using JobPortalAPI.Models;

public interface IAdminRequestRepository
{
    Task<IEnumerable<AdminRequest>> GetPendingRequestsAsync();
    Task<AdminRequest?> GetRequestByIdAsync(int id);
    Task AddRequestAsync(AdminRequest request);
    Task ApproveRequestAsync(AdminRequest request);
    Task RejectRequestAsync(AdminRequest request);
    Task UpdateRequestAsync(AdminRequest request);
}
