using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IAdminActionLogRepository : IRepository<AdminActionLog>
    {
        Task<IEnumerable<AdminActionLog>> GetRecentAsync(int count);
    }
}
