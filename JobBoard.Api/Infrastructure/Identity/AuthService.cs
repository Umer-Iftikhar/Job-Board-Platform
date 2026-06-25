using JobBoard.Api.Application.DTOs;
using JobBoard.Api.Application.DTOs.Auth;
using JobBoard.Api.Application.Interfaces;
using JobBoard.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace JobBoard.Api.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IEmployerProfileService _employerProfileService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IEmployerProfileService employerProfileService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _employerProfileService = employerProfileService;
        _refreshTokenRepository = refreshTokenRepository;
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

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return null;

        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!validPassword) return null;

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);
        if (refreshToken == null || !refreshToken.IsActive)
            return null;

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(refreshToken);

        return await GenerateAuthResponseAsync(refreshToken.User);
    }

    public async Task<bool> RevokeTokenAsync(RefreshTokenDto dto)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);
        if (refreshToken == null || !refreshToken.IsActive)
            return false;

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
        return true;
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateToken(user.Id, user.Email ?? string.Empty, roles);
        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(15);

        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email!,
            ExpiresAt = accessTokenExpiry
        };
    }

    private async Task<string> GenerateRefreshTokenAsync(string userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        return token;
    }
}