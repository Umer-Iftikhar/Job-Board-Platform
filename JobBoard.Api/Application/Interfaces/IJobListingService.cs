using JobBoard.Api.Application.DTOs;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IJobListingService
    {
        Task<PagedResult<JobListingDto>> GetAllActiveAsync(PaginationParams pagination, JobListingSortParams sort);
        Task<JobListingDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<JobListingDto>> GetByEmployerAsync(Guid employerProfileId);
        Task<JobListingDto> CreateAsync(Guid employerProfileId, CreateJobListingDto dto);
        Task<JobListingDto?> UpdateAsync(Guid id, UpdateJobListingDto dto, string userId);
        Task<bool> DeleteAsync(Guid id, string userId);
        Task<bool> IsOwnedByEmployerAsync(Guid id, string userId);

    }
}
