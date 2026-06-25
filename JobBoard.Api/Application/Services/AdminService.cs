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
        private readonly IAdminActionLogRepository _logRepository;

        public AdminService(
            IJobListingRepository jobListingRepository,
            IEmployerProfileRepository employerProfileRepository,
            IAdminActionLogRepository logRepository)
        {
            _jobListingRepository = jobListingRepository;
            _employerProfileRepository = employerProfileRepository;
            _logRepository = logRepository;
        }

        public async Task<PagedResult<JobListingDto>> GetAllJobListingsAsync(PaginationParams pagination, bool includeDeleted = false)
        {
            var allListings = await _jobListingRepository.GetAllWithEmployerAsync(includeDeleted);
            var totalCount = allListings.Count();

            var items = allListings
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(MapToDto)
                .ToList();

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
            var listing = await _jobListingRepository.GetByIdAsync(id);
            if (listing == null) return false;

            await _jobListingRepository.HardDeleteAsync(id);
            return true;
        }

        public async Task<bool> BanEmployerAsync(Guid employerProfileId)
        {
            var employer = await _employerProfileRepository.GetByIdAsync(employerProfileId);
            if (employer == null) return false;

            await _jobListingRepository.SoftDeleteByEmployerAsync(employerProfileId);
            return true;
        }

        public async Task<IEnumerable<AdminActionLogDto>> GetActionLogsAsync(int count = 100)
        {
            var logs = await _logRepository.GetRecentAsync(count);
            return logs.Select(l => new AdminActionLogDto
            {
                Id = l.Id,
                AdminUserId = l.AdminUserId,
                Action = l.Action,
                EntityType = l.EntityType,
                EntityId = l.EntityId,
                Details = l.Details,
                ActionDate = l.ActionDate
            });
        }

        private static JobListingDto MapToDto(JobListing listing)
        {
            return new JobListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Type = listing.Type.ToString(),
                Experience = listing.Experience.ToString(),
                Location = listing.Location,
                SalaryMin = listing.SalaryMin,
                SalaryMax = listing.SalaryMax,
                IsActive = listing.IsActive,
                CreatedAt = listing.CreatedAt,
                UpdatedAt = listing.UpdatedAt,
                Employer = listing.EmployerProfile == null ? null! : new EmployerProfileSummaryDto
                {
                    Id = listing.EmployerProfile.Id,
                    CompanyName = listing.EmployerProfile.CompanyName,
                    Website = listing.EmployerProfile.Website,
                    Location = listing.EmployerProfile.Location
                }
            };
        }
    }
}
