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
                .Where(j => j.IsActive && !j.IsDeleted)
                .Include(j => j.EmployerProfile)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobListing>> GetByEmployerAsync(Guid employerProfileId)
        {
            return await _context.JobListings
                .Where(j => j.EmployerProfileId == employerProfileId && !j.IsDeleted)
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
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
        }

        public async Task<IEnumerable<JobListing>> GetAllWithEmployerAsync(bool includeDeleted)
        {
            var query = _context.JobListings
                .Include(j => j.EmployerProfile)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(j => !j.IsDeleted);

            return await query
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task HardDeleteAsync(Guid id)
        {
            var listing = await _context.JobListings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(j => j.Id == id);

            if (listing != null)
            {
                _context.JobListings.Remove(listing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteByEmployerAsync(Guid employerProfileId)
        {
            var listings = await _context.JobListings
                .Where(j => j.EmployerProfileId == employerProfileId && !j.IsDeleted)
                .ToListAsync();

            foreach (var listing in listings)
            {
                listing.IsDeleted = true;
                listing.DeletedAt = DateTime.UtcNow;
                listing.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
