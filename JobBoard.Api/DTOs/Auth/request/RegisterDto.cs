namespace JobBoard.Api.DTOs.Auth.request
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;   // "Employer" or "Admin"
        public string? FullName { get; set; }
        public string? CompanyName { get; set; }
    }
}
