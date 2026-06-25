namespace JobBoard.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string email, string password, string role, string? fullName = null, string? companyName = null);
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
    }
}
