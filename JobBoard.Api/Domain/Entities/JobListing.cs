using JobBoard.Api.Domain.Enums;

namespace JobBoard.Api.Domain.Entities
{
    public class JobListing : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobType Type { get; set; }
        public ExperienceLevel Experience { get; set; }
        public string? Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid EmployerProfileId { get; set; }
        public EmployerProfile EmployerProfile { get; set; } = null!;

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
