using FluentAssertions;
using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Application.Services;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Domain.Enums;
using Moq;


namespace JobBoard.Api.Tests.Tests
{
    public class JobApplicationServiceTests
    {
        private readonly Mock<IJobApplicationRepository> _appRepo;
        private readonly Mock<IJobListingRepository> _jobRepo;
        private readonly Mock<ICandidateService> _candidateService;
        private readonly Mock<IResumeService> _resumeService;
        private readonly Mock<IEmployerProfileRepository> _employerRepo;
        private readonly Mock<IBackgroundTaskQueue> _taskQueue;
        private readonly Mock<IEmailService> _emailService;
        private readonly JobApplicationService _sut;

        public JobApplicationServiceTests()
        {
            _appRepo = new Mock<IJobApplicationRepository>();
            _jobRepo = new Mock<IJobListingRepository>();
            _candidateService = new Mock<ICandidateService>();
            _resumeService = new Mock<IResumeService>();
            _employerRepo = new Mock<IEmployerProfileRepository>();
            _taskQueue = new Mock<IBackgroundTaskQueue>();
            _emailService = new Mock<IEmailService>();
            _sut = new JobApplicationService(
                _appRepo.Object,
                _jobRepo.Object,
                _candidateService.Object,
                _resumeService.Object,
                _employerRepo.Object,
                _taskQueue.Object,
                _emailService.Object);
        }

        [Fact]
        public async Task CreateAsync_NewApplication_ReturnsDto()
        {
            var dto = new CreateJobApplicationDto
            {
                JobListingId = Guid.NewGuid(),
                CandidateName = "John",
                CandidateEmail = "john@example.com",
                CoverLetter = "Hire me"
            };
            var candidate = new CandidateDto { Id = Guid.NewGuid(), Name = "John", Email = "john@example.com" };
            var resume = new ResumeDto { Id = Guid.NewGuid(), FileName = "cv.pdf", FileUrl = "/resumes/cv.pdf" };
            var createdApplicationId = Guid.NewGuid();

            _candidateService.Setup(x => x.GetOrCreateAsync(It.IsAny<CreateCandidateDto>())).ReturnsAsync(candidate);
            _appRepo.Setup(x => x.ExistsByCandidateAndJobAsync(candidate.Id, dto.JobListingId)).ReturnsAsync(false);
            _resumeService.Setup(x => x.CreateAsync(candidate.Id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>())).ReturnsAsync(resume);
            _appRepo.Setup(x => x.AddAsync(It.IsAny<JobApplication>())).ReturnsAsync((JobApplication a) => { a.Id = createdApplicationId; return a; });

            // Mock the reload with fully populated navigation properties
            _appRepo.Setup(x => x.GetByIdWithDetailsAsync(createdApplicationId)).ReturnsAsync(new JobApplication
            {
                Id = createdApplicationId,
                CandidateId = candidate.Id,
                Candidate = new Candidate { Id = candidate.Id, Name = candidate.Name, Email = candidate.Email },
                JobListingId = dto.JobListingId,
                JobListing = new JobListing
                {
                    Id = dto.JobListingId,
                    Title = "Dev",
                    EmployerProfile = new EmployerProfile { CompanyName = "Acme" }
                },
                ResumeId = resume.Id,
                Resume = new Resume { Id = resume.Id, FileName = resume.FileName, FileUrl = resume.FileUrl },
                Status = ApplicationStatus.Submitted,
                AppliedDate = DateTime.UtcNow
            });

            var result = await _sut.CreateAsync(dto, "/resumes/cv.pdf");

            result.Should().NotBeNull();
            result.Candidate.Email.Should().Be(candidate.Email);
        }

        [Fact]
        public async Task CreateAsync_DuplicateApplication_ThrowsInvalidOperationException()
        {
            var dto = new CreateJobApplicationDto
            {
                JobListingId = Guid.NewGuid(),
                CandidateName = "John",
                CandidateEmail = "john@example.com"
            };
            var candidate = new CandidateDto { Id = Guid.NewGuid(), Name = "John", Email = "john@example.com" };

            _candidateService.Setup(x => x.GetOrCreateAsync(It.IsAny<CreateCandidateDto>())).ReturnsAsync(candidate);
            _appRepo.Setup(x => x.ExistsByCandidateAndJobAsync(candidate.Id, dto.JobListingId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto, "/resumes/cv.pdf"));
        }

        [Fact]
        public async Task UpdateStatusAsync_OwnedApplication_ReturnsUpdatedDto()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var application = new JobApplication
            {
                Id = id,
                JobListing = new JobListing { EmployerProfileId = employer.Id, EmployerProfile = employer },
                Candidate = new Candidate(),
                Resume = new Resume(),
                Status = ApplicationStatus.Submitted
            };
            var dto = new UpdateApplicationStatusDto { Status = ApplicationStatus.Interview };

            _appRepo.Setup(x => x.GetByIdWithDetailsAsync(id)).ReturnsAsync(application);
            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);
            _appRepo.Setup(x => x.UpdateAsync(It.IsAny<JobApplication>())).Returns(Task.CompletedTask);

            var result = await _sut.UpdateStatusAsync(id, dto, userId);

            result.Should().NotBeNull();
            result!.Status.Should().Be(ApplicationStatus.Interview.ToString());
        }

        [Fact]
        public async Task UpdateStatusAsync_Unauthorized_ReturnsNull()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var application = new JobApplication
            {
                Id = id,
                JobListing = new JobListing { EmployerProfileId = Guid.NewGuid() },
                Candidate = new Candidate(),
                Resume = new Resume()
            };

            _appRepo.Setup(x => x.GetByIdWithDetailsAsync(id)).ReturnsAsync(application);
            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);

            var result = await _sut.UpdateStatusAsync(id, new UpdateApplicationStatusDto { Status = ApplicationStatus.Interview }, userId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task IsOwnedByEmployerAsync_Owned_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var application = new JobApplication
            {
                Id = id,
                JobListing = new JobListing { EmployerProfileId = employer.Id }
            };

            _appRepo.Setup(x => x.GetByIdWithDetailsAsync(id)).ReturnsAsync(application);
            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);

            var result = await _sut.IsOwnedByEmployerAsync(id, userId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateStatusAsync_StatusChanged_EnqueuesEmail()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var application = new JobApplication
            {
                Id = id,
                JobListing = new JobListing { EmployerProfileId = employer.Id, EmployerProfile = employer, Title = "Dev" },
                Candidate = new Candidate { Email = "john@example.com", Name = "John" },
                Resume = new Resume(),
                Status = ApplicationStatus.Submitted
            };
            var dto = new UpdateApplicationStatusDto { Status = ApplicationStatus.Interview };

            _appRepo.Setup(x => x.GetByIdWithDetailsAsync(id)).ReturnsAsync(application);
            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);
            _appRepo.Setup(x => x.UpdateAsync(It.IsAny<JobApplication>())).Returns(Task.CompletedTask);
            _taskQueue.Setup(x => x.QueueAsync(It.IsAny<Func<CancellationToken, ValueTask>>())).Returns(ValueTask.CompletedTask);

            var result = await _sut.UpdateStatusAsync(id, dto, userId);

            result.Should().NotBeNull();
            _taskQueue.Verify(x => x.QueueAsync(It.IsAny<Func<CancellationToken, ValueTask>>()), Times.Once);
        }
    }
}

