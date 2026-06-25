using FluentAssertions;
using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Domain.Enums;
using Moq;

namespace JobBoard.Api.Application.Services;

public class AdminServiceTests
{
    private readonly Mock<IJobListingRepository> _jobListingRepo;
    private readonly Mock<IEmployerProfileRepository> _employerRepo;
    private readonly Mock<IAdminActionLogRepository> _logRepo;
    private readonly AdminService _sut;

    public AdminServiceTests()
    {
        _jobListingRepo = new Mock<IJobListingRepository>();
        _employerRepo = new Mock<IEmployerProfileRepository>();
        _logRepo = new Mock<IAdminActionLogRepository>();
        _sut = new AdminService(_jobListingRepo.Object, _employerRepo.Object, _logRepo.Object);
    }

    [Fact]
    public async Task GetAllJobListingsAsync_ExcludesDeletedByDefault()
    {
        var employer = new EmployerProfile { Id = Guid.NewGuid(), CompanyName = "Acme", UserId = "user-1" };
        var listings = new List<JobListing>
        {
            new() { Id = Guid.NewGuid(), Title = "Active", IsDeleted = false, EmployerProfile = employer, Type = JobType.FullTime, Experience = ExperienceLevel.Mid },
            new() { Id = Guid.NewGuid(), Title = "Deleted", IsDeleted = true, DeletedAt = DateTime.UtcNow, EmployerProfile = employer, Type = JobType.FullTime, Experience = ExperienceLevel.Mid }
        };
        _jobListingRepo.Setup(x => x.GetAllWithEmployerAsync(false)).ReturnsAsync(listings.Where(j => !j.IsDeleted));

        var result = await _sut.GetAllJobListingsAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        result.Items.Should().HaveCount(1);
        result.Items.First().Title.Should().Be("Active");
    }

    [Fact]
    public async Task GetAllJobListingsAsync_IncludeDeleted_ReturnsAll()
    {
        var employer = new EmployerProfile { Id = Guid.NewGuid(), CompanyName = "Acme", UserId = "user-1" };
        var listings = new List<JobListing>
        {
            new() { Id = Guid.NewGuid(), Title = "Active", IsDeleted = false, EmployerProfile = employer, Type = JobType.FullTime, Experience = ExperienceLevel.Mid },
            new() { Id = Guid.NewGuid(), Title = "Deleted", IsDeleted = true, DeletedAt = DateTime.UtcNow, EmployerProfile = employer, Type = JobType.FullTime, Experience = ExperienceLevel.Mid }
        };
        _jobListingRepo.Setup(x => x.GetAllWithEmployerAsync(true)).ReturnsAsync(listings);

        var result = await _sut.GetAllJobListingsAsync(new PaginationParams { PageNumber = 1, PageSize = 10 }, includeDeleted: true);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task HardDeleteJobListingAsync_Existing_CallsHardDelete()
    {
        var id = Guid.NewGuid();
        var listing = new JobListing { Id = id, Title = "Gone", Type = JobType.FullTime, Experience = ExperienceLevel.Mid };
        _jobListingRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(listing);

        var result = await _sut.HardDeleteJobListingAsync(id);

        result.Should().BeTrue();
        _jobListingRepo.Verify(x => x.HardDeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task HardDeleteJobListingAsync_NonExisting_ReturnsFalse()
    {
        _jobListingRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((JobListing?)null);

        var result = await _sut.HardDeleteJobListingAsync(Guid.NewGuid());

        result.Should().BeFalse();
        _jobListingRepo.Verify(x => x.HardDeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task BanEmployerAsync_Existing_CallsSoftDeleteByEmployer()
    {
        var employerId = Guid.NewGuid();
        var employer = new EmployerProfile { Id = employerId, CompanyName = "BadActor", UserId = "user-1" };
        _employerRepo.Setup(x => x.GetByIdAsync(employerId)).ReturnsAsync(employer);

        var result = await _sut.BanEmployerAsync(employerId);

        result.Should().BeTrue();
        _jobListingRepo.Verify(x => x.SoftDeleteByEmployerAsync(employerId), Times.Once);
    }

    [Fact]
    public async Task BanEmployerAsync_NonExisting_ReturnsFalse()
    {
        _employerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((EmployerProfile?)null);

        var result = await _sut.BanEmployerAsync(Guid.NewGuid());

        result.Should().BeFalse();
        _jobListingRepo.Verify(x => x.SoftDeleteByEmployerAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetActionLogsAsync_ReturnsRecentLogs()
    {
        var logs = new List<AdminActionLog>
        {
            new() { Id = Guid.NewGuid(), AdminUserId = "admin-1", Action = "DeletedJobListing", EntityType = "JobListing", EntityId = Guid.NewGuid().ToString(), ActionDate = DateTime.UtcNow.AddMinutes(-5) },
            new() { Id = Guid.NewGuid(), AdminUserId = "admin-1", Action = "BannedEmployer", EntityType = "EmployerProfile", EntityId = Guid.NewGuid().ToString(), ActionDate = DateTime.UtcNow }
        };
        _logRepo.Setup(x => x.GetRecentAsync(10)).ReturnsAsync(logs);

        var result = await _sut.GetActionLogsAsync(10);

        result.Should().HaveCount(2);
        result.First().Action.Should().Be("BannedEmployer");
    }
}
