using FluentAssertions;
using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Application.Services;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBoard.Api.Tests.Tests
{
    public class JobListingServiceTests
    {
        private readonly Mock<IJobListingRepository> _jobListingRepo;
        private readonly Mock<IEmployerProfileRepository> _employerRepo;
        private readonly JobListingService _sut;

        public JobListingServiceTests()
        {
            _jobListingRepo = new Mock<IJobListingRepository>();
            _employerRepo = new Mock<IEmployerProfileRepository>();
            _sut = new JobListingService(_jobListingRepo.Object, _employerRepo.Object);
        }

        [Fact]
        public async Task GetAllActiveAsync_ReturnsActiveListings()
        {
            var listings = new List<JobListing>
        {
            new() { Id = Guid.NewGuid(), Title = "Dev", IsActive = true, EmployerProfile = new EmployerProfile { CompanyName = "Acme" } }
        };
            _jobListingRepo.Setup(x => x.GetActiveWithEmployerAsync()).ReturnsAsync(listings);

            var result = await _sut.GetAllActiveAsync();

            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Dev");
        }

        [Fact]
        public async Task GetByIdAsync_Existing_ReturnsDto()
        {
            var id = Guid.NewGuid();
            var listing = new JobListing { Id = id, Title = "Dev", EmployerProfile = new EmployerProfile { CompanyName = "Acme" } };
            _jobListingRepo.Setup(x => x.GetByIdWithApplicationsAsync(id)).ReturnsAsync(listing);

            var result = await _sut.GetByIdAsync(id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetByIdAsync_NonExisting_ReturnsNull()
        {
            _jobListingRepo.Setup(x => x.GetByIdWithApplicationsAsync(It.IsAny<Guid>())).ReturnsAsync((JobListing?)null);

            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsCreatedListing()
        {
            var employerId = Guid.NewGuid();
            var dto = new CreateJobListingDto { Title = "Dev", Description = "Code", Type = JobType.FullTime, Experience = ExperienceLevel.Mid };
            _jobListingRepo.Setup(x => x.AddAsync(It.IsAny<JobListing>())).ReturnsAsync((JobListing j) => j);

            var result = await _sut.CreateAsync(employerId, dto);

            result.Title.Should().Be(dto.Title);
            result.Type.Should().Be(JobType.FullTime.ToString());
        }

        [Fact]
        public async Task UpdateAsync_OwnedListing_ReturnsUpdatedDto()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var listing = new JobListing { Id = id, EmployerProfileId = employer.Id, Title = "Old" };
            var dto = new UpdateJobListingDto { Title = "New", Description = "Desc", Type = JobType.FullTime, Experience = ExperienceLevel.Mid, IsActive = true };

            _jobListingRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(listing);
            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);
            _jobListingRepo.Setup(x => x.UpdateAsync(It.IsAny<JobListing>())).Returns(Task.CompletedTask);

            var result = await _sut.UpdateAsync(id, dto, userId);

            result.Should().NotBeNull();
            result!.Title.Should().Be("New");
        }

        [Fact]
        public async Task UpdateAsync_UnauthorizedListing_ReturnsNull()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var listing = new JobListing { Id = id, EmployerProfileId = Guid.NewGuid(), Title = "Old" };
            var dto = new UpdateJobListingDto { Title = "New", Description = "Desc", Type = JobType.FullTime, Experience = ExperienceLevel.Mid, IsActive = true };

            _jobListingRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(listing);
            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);

            var result = await _sut.UpdateAsync(id, dto, userId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_OwnedListing_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var listing = new JobListing { Id = id, EmployerProfileId = employer.Id };

            _jobListingRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(listing);
            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);
            _jobListingRepo.Setup(x => x.DeleteAsync(It.IsAny<JobListing>())).Returns(Task.CompletedTask);

            var result = await _sut.DeleteAsync(id, userId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            _jobListingRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((JobListing?)null);

            var result = await _sut.DeleteAsync(Guid.NewGuid(), "user");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsOwnedByEmployerAsync_Owned_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            var userId = "user-123";
            var employer = new EmployerProfile { Id = Guid.NewGuid(), UserId = userId };
            var listing = new JobListing { Id = id, EmployerProfileId = employer.Id };

            _employerRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(employer);
            _jobListingRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(listing);

            var result = await _sut.IsOwnedByEmployerAsync(id, userId);

            result.Should().BeTrue();
        }
    }
}
