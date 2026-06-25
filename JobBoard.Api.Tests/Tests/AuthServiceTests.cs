using FluentAssertions;  
using JobBoard.Api.Application.DTOs;  
using JobBoard.Api.Application.DTOs.Auth;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using JobBoard.Api.Infrastructure.Identity;
using JobBoard.Api.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;



namespace JobBoard.Api.Tests.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<IJwtService> _jwtService;
        private readonly Mock<IEmployerProfileService> _employerProfileService;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _userManager = MockUserManager.Create<ApplicationUser>();
            _jwtService = new Mock<IJwtService>();
            _employerProfileService = new Mock<IEmployerProfileService>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _sut = new AuthService(
                _userManager.Object,
                _jwtService.Object,
                _employerProfileService.Object,
                _refreshTokenRepository.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithNewEmail_ReturnsAuthResponse()
        {
            var dto = new RegisterDto { Email = "test@example.com", Password = "Password123!", CompanyName = "Acme" };
            _userManager.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);
            _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Employer"))
                .ReturnsAsync(IdentityResult.Success);
            _jwtService.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns("access-token");

            var result = await _sut.RegisterAsync(dto);

            result.Should().NotBeNull();
            result!.Token.Should().Be("access-token");
            _employerProfileService.Verify(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<CreateEmployerProfileDto>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ReturnsNull()
        {
            var dto = new RegisterDto { Email = "test@example.com", Password = "Password123!", CompanyName = "Acme" };
            _userManager.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(new ApplicationUser());

            var result = await _sut.RegisterAsync(dto);

            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
        {
            var dto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { Id = "user-id", Email = dto.Email };
            _userManager.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Employer" });
            _jwtService.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns("access-token");

            var result = await _sut.LoginAsync(dto);

            result.Should().NotBeNull();
            result!.Token.Should().Be("access-token");
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ReturnsNull()
        {
            var dto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            _userManager.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

            var result = await _sut.LoginAsync(dto);

            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ReturnsNull()
        {
            var dto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { Id = "user-id", Email = dto.Email };
            _userManager.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            var result = await _sut.LoginAsync(dto);

            result.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_WithValidToken_ReturnsNewAuthResponse()
        {
            var dto = new RefreshTokenDto { RefreshToken = "valid-refresh-token" };
            var user = new ApplicationUser { Id = "user-id", Email = "test@example.com" };
            var refreshToken = new RefreshToken { Token = dto.RefreshToken, User = user, ExpiresAt = DateTime.UtcNow.AddDays(1), IsRevoked = false };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(dto.RefreshToken)).ReturnsAsync(refreshToken);
            _refreshTokenRepository.Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            _jwtService.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns("new-access-token");

            var result = await _sut.RefreshTokenAsync(dto);

            result.Should().NotBeNull();
            result!.Token.Should().Be("new-access-token");
            refreshToken.IsRevoked.Should().BeTrue();
        }

        [Fact]
        public async Task RefreshTokenAsync_WithInvalidToken_ReturnsNull()
        {
            var dto = new RefreshTokenDto { RefreshToken = "invalid-token" };
            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(dto.RefreshToken)).ReturnsAsync((RefreshToken?)null);

            var result = await _sut.RefreshTokenAsync(dto);

            result.Should().BeNull();
        }

        [Fact]
        public async Task RevokeTokenAsync_WithValidToken_ReturnsTrue()
        {
            var dto = new RefreshTokenDto { RefreshToken = "valid-token" };
            var token = new RefreshToken { Token = dto.RefreshToken, IsRevoked = false, ExpiresAt = DateTime.UtcNow.AddDays(1) };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(dto.RefreshToken)).ReturnsAsync(token);
            _refreshTokenRepository.Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

            var result = await _sut.RevokeTokenAsync(dto);

            result.Should().BeTrue();
            token.IsRevoked.Should().BeTrue();
        }

        [Fact]
        public async Task RevokeTokenAsync_WithRevokedToken_ReturnsFalse()
        {
            var dto = new RefreshTokenDto { RefreshToken = "revoked-token" };
            var token = new RefreshToken { Token = dto.RefreshToken, IsRevoked = true };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(dto.RefreshToken)).ReturnsAsync(token);

            var result = await _sut.RevokeTokenAsync(dto);

            result.Should().BeFalse();
        }
    }
}
