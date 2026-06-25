using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;

namespace JobBoard.Api.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IJobListingRepository _jobListingRepository;
        private readonly ICandidateService _candidateService;
        private readonly IResumeService _resumeService;
        private readonly IEmployerProfileRepository _employerRepository;

        public JobApplicationService(
            IJobApplicationRepository repository,
            IJobListingRepository jobListingRepository,
            ICandidateService candidateService,
            IResumeService resumeService,
            IEmployerProfileRepository employerRepository)
        {
            _repository = repository;
            _jobListingRepository = jobListingRepository;
            _candidateService = candidateService;
            _resumeService = resumeService;
            _employerRepository = employerRepository;
        }

        public async Task<IEnumerable<JobApplicationDto>> GetByJobListingAsync(Guid jobListingId, string userId)
        {
            var employer = await _employerRepository.GetByUserIdAsync(userId);
            if (employer == null) return Enumerable.Empty<JobApplicationDto>();

            var listing = await _jobListingRepository.GetByIdAsync(jobListingId);
            if (listing == null || listing.EmployerProfileId != employer.Id)
                return Enumerable.Empty<JobApplicationDto>();

            var applications = await _repository.GetByJobListingAsync(jobListingId);
            return applications.Select(MapToDto);
        }

        public async Task<IEnumerable<JobApplicationDto>> GetByCandidateAsync(Guid candidateId)
        {
            var applications = await _repository.GetByCandidateAsync(candidateId);
            return applications.Select(MapToDto);
        }

        public async Task<JobApplicationDto?> GetByIdAsync(Guid id)
        {
            var application = await _repository.GetByIdWithDetailsAsync(id);
            return application == null ? null : MapToDto(application);
        }

        public async Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto dto, string resumeFileUrl)
        {
            var candidate = await _candidateService.GetOrCreateAsync(new CreateCandidateDto
            {
                Name = dto.CandidateName,
                Email = dto.CandidateEmail
            });

            var alreadyApplied = await _repository.ExistsByCandidateAndJobAsync(candidate.Id, dto.JobListingId);
            if (alreadyApplied)
                throw new InvalidOperationException("Candidate has already applied to this job listing.");

            var resume = await _resumeService.CreateAsync(candidate.Id, dto.CandidateName + "_resume", resumeFileUrl, null);

            var application = new JobApplication
            {
                CandidateId = candidate.Id,
                JobListingId = dto.JobListingId,
                ResumeId = resume.Id,
                CoverLetter = dto.CoverLetter
            };

            await _repository.AddAsync(application);

            // Reload with navigation properties for mapping
            var created = await _repository.GetByIdWithDetailsAsync(application.Id)
                ?? throw new InvalidOperationException("Failed to load created application.");

            return MapToDto(created);
        }

        public async Task<JobApplicationDto?> UpdateStatusAsync(Guid id, UpdateApplicationStatusDto dto, string userId)
        {
            var application = await _repository.GetByIdWithDetailsAsync(id);
            if (application == null) return null;

            var employer = await _employerRepository.GetByUserIdAsync(userId);
            if (employer == null || application.JobListing.EmployerProfileId != employer.Id)
                return null;

            application.Status = dto.Status;
            await _repository.UpdateAsync(application);
            return MapToDto(application);
        }

        public async Task<bool> IsOwnedByEmployerAsync(Guid id, string userId)
        {
            var application = await _repository.GetByIdWithDetailsAsync(id);
            if (application == null) return false;

            var employer = await _employerRepository.GetByUserIdAsync(userId);
            if (employer == null) return false;

            return application.JobListing.EmployerProfileId == employer.Id;
        }

        private static JobApplicationDto MapToDto(JobApplication application)
        {
            return new JobApplicationDto
            {
                Id = application.Id,
                Status = application.Status.ToString(),
                AppliedDate = application.AppliedDate,
                CoverLetter = application.CoverLetter,
                Candidate = new CandidateDto
                {
                    Id = application.Candidate.Id,
                    Name = application.Candidate.Name,
                    Email = application.Candidate.Email,
                    CreatedAt = application.Candidate.CreatedAt
                },
                JobListing = application.JobListing == null ? null! : new JobListingSummaryDto
                {
                    Id = application.JobListing.Id,
                    Title = application.JobListing.Title,
                    CompanyName = application.JobListing.EmployerProfile?.CompanyName ?? string.Empty
                },
                Resume = new ResumeDto
                {
                    Id = application.Resume.Id,
                    FileName = application.Resume.FileName,
                    FileUrl = application.Resume.FileUrl,
                    ContentType = application.Resume.ContentType,
                    CreatedAt = application.Resume.CreatedAt
                }
            };
        }
    }
}
