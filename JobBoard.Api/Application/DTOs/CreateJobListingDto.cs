using JobBoard.Api.Domain.Enums;

namespace JobBoard.Api.Application.DTOs
{
    public class CreateJobListingDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobType Type { get; set; }
        public ExperienceLevel Experience { get; set; }
        public string? Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
    }
}
