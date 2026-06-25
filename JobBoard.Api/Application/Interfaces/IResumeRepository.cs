using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IResumeRepository : IRepository<Resume>
    {
        Task<IEnumerable<Resume>> GetByCandidateAsync(Guid candidateId);
    }
}
