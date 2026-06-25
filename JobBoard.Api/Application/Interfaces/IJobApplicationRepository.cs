using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
    {
        Task<IEnumerable<JobApplication>> GetByJobListingAsync(Guid jobListingId);
        Task<IEnumerable<JobApplication>> GetByCandidateAsync(Guid candidateId);
        Task<JobApplication?> GetByIdWithDetailsAsync(Guid id);
        Task<bool> ExistsByCandidateAndJobAsync(Guid candidateId, Guid jobListingId);
    }
}
