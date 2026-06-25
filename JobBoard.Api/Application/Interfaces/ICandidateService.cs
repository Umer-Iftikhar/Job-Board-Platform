using JobBoard.Api.Application.DTOs;

namespace JobBoard.Api.Application.Interfaces
{
    public interface ICandidateService
    {
        Task<CandidateDto?> GetByEmailAsync(string email);
        Task<CandidateDto> GetOrCreateAsync(CreateCandidateDto dto);
        Task<CandidateDto?> GetByIdAsync(Guid id);
    }
}
