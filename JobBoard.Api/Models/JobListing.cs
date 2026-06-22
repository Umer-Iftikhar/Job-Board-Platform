using static System.Net.Mime.MediaTypeNames;

namespace JobBoard.Api.Models
{
    public class JobListing
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }   // EmployerProfile.Id
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public JobType Type { get; set; }
        public ExperienceLevel Experience { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;

        public EmployerProfile Employer { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}
