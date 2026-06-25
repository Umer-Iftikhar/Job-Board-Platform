using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.DTOs.Auth;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Api.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IEmployerProfileService _employerProfileService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            IEmployerProfileService employerProfileService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _employerProfileService = employerProfileService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(user, "Employer");

            await _employerProfileService.CreateAsync(user.Id, new CreateEmployerProfileDto
            {
                CompanyName = dto.CompanyName
            });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user.Id, user.Email, roles);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!validPassword) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user.Id, user.Email!, roles);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };
        }
    }
}
