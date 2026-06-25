using FluentAssertions;  
using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Application.Services;
using JobBoard.Api.Domain.Entities;
using Moq;



namespace JobBoard.Api.Tests.Tests
{
    public class CandidateServiceTests
    {
        private readonly Mock<ICandidateRepository> _repository;
        private readonly CandidateService _sut;

        public CandidateServiceTests()
        {
            _repository = new Mock<ICandidateRepository>();
            _sut = new CandidateService(_repository.Object);
        }

        [Fact]
        public async Task GetByEmailAsync_ExistingCandidate_ReturnsDto()
        {
            var candidate = new Candidate { Id = Guid.NewGuid(), Name = "John", Email = "john@example.com" };
            _repository.Setup(x => x.GetByEmailAsync(candidate.Email)).ReturnsAsync(candidate);

            var result = await _sut.GetByEmailAsync(candidate.Email);

            result.Should().NotBeNull();
            result!.Email.Should().Be(candidate.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_NonExisting_ReturnsNull()
        {
            _repository.Setup(x => x.GetByEmailAsync("missing@example.com")).ReturnsAsync((Candidate?)null);

            var result = await _sut.GetByEmailAsync("missing@example.com");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrCreateAsync_Existing_ReturnsExistingDto()
        {
            var dto = new CreateCandidateDto { Name = "John", Email = "john@example.com" };
            var existing = new Candidate { Id = Guid.NewGuid(), Name = "John", Email = dto.Email };
            _repository.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync(existing);

            var result = await _sut.GetOrCreateAsync(dto);

            result.Email.Should().Be(dto.Email);
            _repository.Verify(x => x.AddAsync(It.IsAny<Candidate>()), Times.Never);
        }

        [Fact]
        public async Task GetOrCreateAsync_New_CreatesAndReturnsDto()
        {
            var dto = new CreateCandidateDto { Name = "John", Email = "john@example.com" };
            _repository.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((Candidate?)null);
            _repository.Setup(x => x.AddAsync(It.IsAny<Candidate>())).ReturnsAsync((Candidate c) => c);

            var result = await _sut.GetOrCreateAsync(dto);

            result.Email.Should().Be(dto.Email);
            _repository.Verify(x => x.AddAsync(It.Is<Candidate>(c => c.Email == dto.Email)), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Existing_ReturnsDto()
        {
            var id = Guid.NewGuid();
            var candidate = new Candidate { Id = id, Name = "John", Email = "john@example.com" };
            _repository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(candidate);

            var result = await _sut.GetByIdAsync(id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetByIdAsync_NonExisting_ReturnsNull()
        {
            _repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Candidate?)null);

            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }
}
