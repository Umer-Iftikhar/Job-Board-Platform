using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Infrastructure.Services
{
    public class EmployerProfileService : IEmployerProfileService
    {
        private readonly IEmployerProfileRepository _repository;

        public EmployerProfileService(IEmployerProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployerProfileDto?> GetByUserIdAsync(string userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);
            return profile == null ? null : MapToDto(profile);
        }

        public async Task<EmployerProfileDto> CreateAsync(string userId, CreateEmployerProfileDto dto)
        {
            var profile = new EmployerProfile
            {
                UserId = userId,
                CompanyName = dto.CompanyName,
                Description = dto.Description,
                Website = dto.Website,
                Location = dto.Location
            };

            await _repository.AddAsync(profile);
            return MapToDto(profile);
        }

        public async Task<EmployerProfileDto?> GetByIdAsync(Guid id)
        {
            var profile = await _repository.GetByIdAsync(id);
            return profile == null ? null : MapToDto(profile);
        }

        public async Task<bool> ExistsForUserAsync(string userId)
        {
            return await _repository.ExistsAsync(e => e.UserId == userId);
        }

        private static EmployerProfileDto MapToDto(EmployerProfile profile)
        {
            return new EmployerProfileDto
            {
                Id = profile.Id,
                CompanyName = profile.CompanyName,
                Description = profile.Description,
                Website = profile.Website,
                Location = profile.Location,
                CreatedAt = profile.CreatedAt
            };
        }
    }
}
