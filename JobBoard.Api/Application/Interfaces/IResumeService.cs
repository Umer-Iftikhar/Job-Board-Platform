using JobBoard.Api.Application.DTOs;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IResumeService
    {
        Task<ResumeDto> CreateAsync(Guid candidateId, string fileName, string fileUrl, string? contentType);
        Task<IEnumerable<ResumeDto>> GetByCandidateAsync(Guid candidateId);
    }
}
