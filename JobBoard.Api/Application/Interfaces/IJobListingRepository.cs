using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IJobListingRepository : IRepository<JobListing>
    {
        Task<IEnumerable<JobListing>> GetActiveWithEmployerAsync();
        Task<IEnumerable<JobListing>> GetByEmployerAsync(Guid employerProfileId);
        Task<JobListing?> GetByIdWithApplicationsAsync(Guid id);
        Task<IEnumerable<JobListing>> GetAllWithEmployerAsync(bool includeDeleted);
        Task HardDeleteAsync(Guid id);
        Task SoftDeleteByEmployerAsync(Guid employerProfileId);
    }
}
