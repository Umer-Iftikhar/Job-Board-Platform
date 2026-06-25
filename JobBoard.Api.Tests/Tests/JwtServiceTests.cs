using FluentAssertions;
using JobBoard.Api.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using JobBoard.Api.Infrastructure.Identity;


namespace JobBoard.Api.Tests.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtService _sut;

        public JwtServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", "this-is-a-super-secret-key-32-chars!"},
            {"Jwt:Issuer", "JobBoard"},
            {"Jwt:Audience", "JobBoard"}
        };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _sut = new JwtService(configuration);
        }

        [Fact]
        public void GenerateToken_ReturnsNonEmptyString()
        {
            var token = _sut.GenerateToken("user-1", "test@example.com", new[] { "Employer" });

            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsPrincipal()
        {
            var token = _sut.GenerateToken("user-1", "test@example.com", new[] { "Employer" });

            var principal = _sut.ValidateToken(token);

            principal.Should().NotBeNull();
            principal!.FindFirst(ClaimTypes.Email)?.Value.Should().Be("test@example.com");
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ReturnsNull()
        {
            var principal = _sut.ValidateToken("invalid-token-string");

            principal.Should().BeNull();
        }
    }
}
