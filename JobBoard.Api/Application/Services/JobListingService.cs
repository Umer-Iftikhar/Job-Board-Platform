using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Services
{
    public class JobListingService : IJobListingService
    {
        private readonly IJobListingRepository _repository;
        private readonly IEmployerProfileRepository _employerRepository;

        public JobListingService(
            IJobListingRepository repository,
            IEmployerProfileRepository employerRepository)
        {
            _repository = repository;
            _employerRepository = employerRepository;
        }

        public async Task<PagedResult<JobListingDto>> GetAllActiveAsync(
        PaginationParams pagination,
        JobListingSortParams sort,
        JobListingFilterParams filter)
        {
            var query = (await _repository.GetActiveWithEmployerAsync())
                .AsQueryable();

            // Search (title or description contains term, case-insensitive)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLowerInvariant();
                query = query.Where(j =>
                    j.Title.ToLower().Contains(term) ||
                    j.Description.ToLower().Contains(term));
            }

            // Filter by type
            if (filter.Type.HasValue)
                query = query.Where(j => j.Type == filter.Type.Value);

            // Filter by experience level
            if (filter.Experience.HasValue)
                query = query.Where(j => j.Experience == filter.Experience.Value);

            // Filter by location
            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                var loc = filter.Location.ToLowerInvariant();
                query = query.Where(j => j.Location != null && j.Location.ToLower().Contains(loc));
            }

            // Remote filter
            if (filter.IsRemote.HasValue)
            {
                if (filter.IsRemote.Value)
                    query = query.Where(j => j.Location == null || j.Location.ToLower().Contains("remote"));
                else
                    query = query.Where(j => j.Location != null && !j.Location.ToLower().Contains("remote"));
            }

            // Salary range
            if (filter.MinSalary.HasValue)
                query = query.Where(j => j.SalaryMax == null || j.SalaryMax >= filter.MinSalary.Value);

            if (filter.MaxSalary.HasValue)
                query = query.Where(j => j.SalaryMin == null || j.SalaryMin <= filter.MaxSalary.Value);

            // Sorting
            query = sort.SortBy?.ToLowerInvariant() switch
            {
                "title" => sort.SortOrder == "asc"
                    ? query.OrderBy(j => j.Title)
                    : query.OrderByDescending(j => j.Title),
                "salarymin" => sort.SortOrder == "asc"
                    ? query.OrderBy(j => j.SalaryMin)
                    : query.OrderByDescending(j => j.SalaryMin),
                "salarymax" => sort.SortOrder == "asc"
                    ? query.OrderBy(j => j.SalaryMax)
                    : query.OrderByDescending(j => j.SalaryMax),
                _ => sort.SortOrder == "asc"
                    ? query.OrderBy(j => j.CreatedAt)
                    : query.OrderByDescending(j => j.CreatedAt)
            };

            var totalCount = query.Count();

            var items = query
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

        public async Task<JobListingDto?> GetByIdAsync(Guid id)
        {
            var listing = await _repository.GetByIdWithApplicationsAsync(id);
            return listing == null ? null : MapToDto(listing);
        }

        public async Task<IEnumerable<JobListingDto>> GetByEmployerAsync(Guid employerProfileId)
        {
            var listings = await _repository.GetByEmployerAsync(employerProfileId);
            return listings.Select(MapToDto);
        }

        public async Task<JobListingDto> CreateAsync(Guid employerProfileId, CreateJobListingDto dto)
        {
            var listing = new JobListing
            {
                EmployerProfileId = employerProfileId,
                Title = dto.Title,
                Description = dto.Description,
                Type = dto.Type,
                Experience = dto.Experience,
                Location = dto.Location,
                SalaryMin = dto.SalaryMin,
                SalaryMax = dto.SalaryMax
            };

            await _repository.AddAsync(listing);
            return MapToDto(listing);
        }

        public async Task<JobListingDto?> UpdateAsync(Guid id, UpdateJobListingDto dto, string userId)
        {
            var listing = await _repository.GetByIdAsync(id);
            if (listing == null) return null;

            var employer = await _employerRepository.GetByUserIdAsync(userId);
            if (employer == null || listing.EmployerProfileId != employer.Id)
                return null;

            listing.Title = dto.Title;
            listing.Description = dto.Description;
            listing.Type = dto.Type;
            listing.Experience = dto.Experience;
            listing.Location = dto.Location;
            listing.SalaryMin = dto.SalaryMin;
            listing.SalaryMax = dto.SalaryMax;
            listing.IsActive = dto.IsActive;

            await _repository.UpdateAsync(listing);
            return MapToDto(listing);
        }

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            var listing = await _repository.GetByIdAsync(id);
            if (listing == null) return false;

            var employer = await _employerRepository.GetByUserIdAsync(userId);
            if (employer == null || listing.EmployerProfileId != employer.Id)
                return false;

            listing.IsDeleted = true;
            listing.DeletedAt = DateTime.UtcNow;
            listing.IsActive = false;

            await _repository.UpdateAsync(listing);
            return true;
        }

        public async Task<bool> IsOwnedByEmployerAsync(Guid id, string userId)
        {
            var employer = await _employerRepository.GetByUserIdAsync(userId);
            if (employer == null) return false;

            var listing = await _repository.GetByIdAsync(id);
            return listing != null && listing.EmployerProfileId == employer.Id;
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
