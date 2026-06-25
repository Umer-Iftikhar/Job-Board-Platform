using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Infrastructure.Repositories
{
    public class EmployerProfileRepository : Repository<EmployerProfile>, IEmployerProfileRepository
    {
        public EmployerProfileRepository(ApplicationDbContext context) : base(context) { }

        public async Task<EmployerProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.EmployerProfiles
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<EmployerProfile?> GetByIdWithJobListingsAsync(Guid id)
        {
            return await _context.EmployerProfiles
                .Include(e => e.JobListings)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }

}
