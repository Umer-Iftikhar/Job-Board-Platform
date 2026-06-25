using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IEmployerProfileRepository : IRepository<EmployerProfile>
    {
        Task<EmployerProfile?> GetByUserIdAsync(string userId);
        Task<EmployerProfile?> GetByIdWithJobListingsAsync(Guid id);
    }
}
