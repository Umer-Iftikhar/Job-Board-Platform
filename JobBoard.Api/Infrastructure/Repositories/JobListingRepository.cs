using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Infrastructure.Repositories
{
    public class JobListingRepository : Repository<JobListing>, IJobListingRepository
    {
        public JobListingRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<JobListing>> GetActiveWithEmployerAsync()
        {
            return await _context.JobListings
                .Where(j => j.IsActive)
                .Include(j => j.EmployerProfile)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobListing>> GetByEmployerAsync(Guid employerProfileId)
        {
            return await _context.JobListings
                .Where(j => j.EmployerProfileId == employerProfileId)
                .Include(j => j.EmployerProfile)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<JobListing?> GetByIdWithApplicationsAsync(Guid id)
        {
            return await _context.JobListings
                .Include(j => j.EmployerProfile)
                .Include(j => j.Applications)
                .ThenInclude(a => a.Candidate)
                .FirstOrDefaultAsync(j => j.Id == id);
        }
    }
}
