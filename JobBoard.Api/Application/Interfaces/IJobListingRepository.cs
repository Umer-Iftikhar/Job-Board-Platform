using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IJobListingRepository : IRepository<JobListing>
    {
        Task<IEnumerable<JobListing>> GetActiveWithEmployerAsync();
        Task<IEnumerable<JobListing>> GetByEmployerAsync(Guid employerProfileId);
        Task<JobListing?> GetByIdWithApplicationsAsync(Guid id);
    }
}
