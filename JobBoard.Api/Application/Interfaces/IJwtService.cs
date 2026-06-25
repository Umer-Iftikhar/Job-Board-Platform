using System.Security.Claims;

namespace JobBoard.Api.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, IEnumerable<string> roles);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
