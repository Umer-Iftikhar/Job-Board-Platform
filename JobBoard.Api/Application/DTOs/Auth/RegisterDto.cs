namespace JobBoard.Api.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
