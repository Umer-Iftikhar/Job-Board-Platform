using JobBoard.Api.Application.DTOs;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IEmployerProfileService
    {
        Task<EmployerProfileDto?> GetByUserIdAsync(string userId);
        Task<EmployerProfileDto> CreateAsync(string userId, CreateEmployerProfileDto dto);
        Task<EmployerProfileDto?> GetByIdAsync(Guid id);
        Task<bool> ExistsForUserAsync(string userId);
    }
}
