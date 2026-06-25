using FluentAssertions;
using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Services;
using Moq;


namespace JobBoard.Api.Tests.Tests
{
    public class EmployerProfileServiceTests
    {
        private readonly Mock<IEmployerProfileRepository> _repository;
        private readonly EmployerProfileService _sut;

        public EmployerProfileServiceTests()
        {
            _repository = new Mock<IEmployerProfileRepository>();
            _sut = new EmployerProfileService(_repository.Object);
        }

        [Fact]
        public async Task GetByUserIdAsync_Existing_ReturnsDto()
        {
            var profile = new EmployerProfile { Id = Guid.NewGuid(), UserId = "user-1", CompanyName = "Acme" };
            _repository.Setup(x => x.GetByUserIdAsync("user-1")).ReturnsAsync(profile);

            var result = await _sut.GetByUserIdAsync("user-1");

            result.Should().NotBeNull();
            result!.CompanyName.Should().Be("Acme");
        }

        [Fact]
        public async Task GetByUserIdAsync_NonExisting_ReturnsNull()
        {
            _repository.Setup(x => x.GetByUserIdAsync("user-1")).ReturnsAsync((EmployerProfile?)null);

            var result = await _sut.GetByUserIdAsync("user-1");

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsDto()
        {
            var userId = "user-1";
            var dto = new CreateEmployerProfileDto { CompanyName = "Acme", Description = "We build things" };
            _repository.Setup(x => x.AddAsync(It.IsAny<EmployerProfile>())).ReturnsAsync((EmployerProfile p) => p);

            var result = await _sut.CreateAsync(userId, dto);

            result.CompanyName.Should().Be("Acme");
        }

        [Fact]
        public async Task ExistsForUserAsync_Existing_ReturnsTrue()
        {
            _repository.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EmployerProfile, bool>>>())).ReturnsAsync(true);

            var result = await _sut.ExistsForUserAsync("user-1");

            result.Should().BeTrue();
        }
    }
}
