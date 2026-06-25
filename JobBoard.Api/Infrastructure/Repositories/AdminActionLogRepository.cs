using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Infrastructure.Repositories
{
    public class AdminActionLogRepository : Repository<AdminActionLog>, IAdminActionLogRepository
    {
        public AdminActionLogRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<AdminActionLog>> GetRecentAsync(int count)
        {
            return await _context.AdminActionLogs
                .OrderByDescending(l => l.ActionDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
