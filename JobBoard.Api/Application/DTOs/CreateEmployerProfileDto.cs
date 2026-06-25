namespace JobBoard.Api.Application.DTOs
{
    public class CreateEmployerProfileDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
    }
}
