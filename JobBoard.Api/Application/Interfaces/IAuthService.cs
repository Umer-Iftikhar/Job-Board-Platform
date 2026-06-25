using JobBoard.Api.Application.DTOs.Auth;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto);
        Task<bool> RevokeTokenAsync(RefreshTokenDto dto);
    }
}
