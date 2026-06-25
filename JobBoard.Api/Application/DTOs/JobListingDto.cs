namespace JobBoard.Api.Application.DTOs
{
    public class JobListingDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string? Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EmployerProfileSummaryDto Employer { get; set; } = null!;
    }

    public class EmployerProfileSummaryDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Location { get; set; }
    }
}
