using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IJobListingRepository _jobListingRepository;
        private readonly IEmployerProfileRepository _employerProfileRepository;
        private readonly IRepository<AdminActionLog> _logRepository;
        private readonly ApplicationDbContext _context;

        public AdminService(
            IJobListingRepository jobListingRepository,
            IEmployerProfileRepository employerProfileRepository,
            IRepository<AdminActionLog> logRepository,
            ApplicationDbContext context)
        {
            _jobListingRepository = jobListingRepository;
            _employerProfileRepository = employerProfileRepository;
            _logRepository = logRepository;
            _context = context;
        }

        public async Task<PagedResult<JobListingDto>> GetAllJobListingsAsync(PaginationParams pagination, bool includeDeleted = false)
        {
            var query = _context.JobListings
                .Include(j => j.EmployerProfile)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(j => !j.IsDeleted);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(j => new JobListingDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Type = j.Type.ToString(),
                    Experience = j.Experience.ToString(),
                    Location = j.Location,
                    SalaryMin = j.SalaryMin,
                    SalaryMax = j.SalaryMax,
                    IsActive = j.IsActive,
                    CreatedAt = j.CreatedAt,
                    UpdatedAt = j.UpdatedAt,
                    Employer = new EmployerProfileSummaryDto
                    {
                        Id = j.EmployerProfile.Id,
                        CompanyName = j.EmployerProfile.CompanyName,
                        Website = j.EmployerProfile.Website,
                        Location = j.EmployerProfile.Location
                    }
                })
                .ToListAsync();

            return new PagedResult<JobListingDto>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> HardDeleteJobListingAsync(Guid id)
        {
            var listing = await _context.JobListings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(j => j.Id == id);

            if (listing == null) return false;

            _context.JobListings.Remove(listing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BanEmployerAsync(Guid employerProfileId)
        {
            var employer = await _employerProfileRepository.GetByIdAsync(employerProfileId);
            if (employer == null) return false;

            // Soft delete all their job listings
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
            return true;
        }

        public async Task<IEnumerable<AdminActionLogDto>> GetActionLogsAsync(int count = 100)
        {
            return await _context.AdminActionLogs
                .OrderByDescending(l => l.ActionDate)
                .Take(count)
                .Select(l => new AdminActionLogDto
                {
                    Id = l.Id,
                    AdminUserId = l.AdminUserId,
                    Action = l.Action,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Details = l.Details,
                    ActionDate = l.ActionDate
                })
                .ToListAsync();
        }
    }
}
