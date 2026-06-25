using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Interfaces
{
    public interface ICandidateRepository : IRepository<Candidate>
    {
        Task<Candidate?> GetByEmailAsync(string email);
        Task<Candidate?> GetByEmailWithApplicationsAsync(string email);
    }
}
