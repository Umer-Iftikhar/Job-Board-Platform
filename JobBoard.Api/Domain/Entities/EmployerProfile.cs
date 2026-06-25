namespace JobBoard.Api.Domain.Entities
{
    public class EmployerProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }

        public ICollection<JobListing> JobListings { get; set; } = new List<JobListing>();
    }
}
