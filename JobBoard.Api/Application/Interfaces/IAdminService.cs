using JobBoard.Api.Application.DTOs;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IAdminService
    {
        Task<PagedResult<JobListingDto>> GetAllJobListingsAsync(PaginationParams pagination, bool includeDeleted = false);
        Task<bool> HardDeleteJobListingAsync(Guid id);
        Task<bool> BanEmployerAsync(Guid employerProfileId);
        Task<IEnumerable<AdminActionLogDto>> GetActionLogsAsync(int count = 100);
    }
}
