using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _repository;

        public CandidateService(ICandidateRepository repository)
        {
            _repository = repository;
        }

        public async Task<CandidateDto?> GetByEmailAsync(string email)
        {
            var candidate = await _repository.GetByEmailAsync(email);
            return candidate == null ? null : MapToDto(candidate);
        }

        public async Task<CandidateDto> GetOrCreateAsync(CreateCandidateDto dto)
        {
            var existing = await _repository.GetByEmailAsync(dto.Email);
            if (existing != null)
                return MapToDto(existing);

            var candidate = new Candidate
            {
                Name = dto.Name,
                Email = dto.Email
            };

            await _repository.AddAsync(candidate);
            return MapToDto(candidate);
        }

        public async Task<CandidateDto?> GetByIdAsync(Guid id)
        {
            var candidate = await _repository.GetByIdAsync(id);
            return candidate == null ? null : MapToDto(candidate);
        }

        private static CandidateDto MapToDto(Candidate candidate)
        {
            return new CandidateDto
            {
                Id = candidate.Id,
                Name = candidate.Name,
                Email = candidate.Email,
                CreatedAt = candidate.CreatedAt
            };
        }
    }
}
