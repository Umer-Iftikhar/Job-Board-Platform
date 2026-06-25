using JobBoard.Api.Application.DTOs;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IJobApplicationService
    {
        Task<IEnumerable<JobApplicationDto>> GetByJobListingAsync(Guid jobListingId, string userId);
        Task<IEnumerable<JobApplicationDto>> GetByCandidateAsync(Guid candidateId);
        Task<JobApplicationDto?> GetByIdAsync(Guid id);
        Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto dto, string resumeFileUrl);
        Task<JobApplicationDto?> UpdateStatusAsync(Guid id, UpdateApplicationStatusDto dto, string userId);
        Task<bool> IsOwnedByEmployerAsync(Guid id, string userId);
    }
}
