using JobBoard.Api.Domain.Enums;

namespace JobBoard.Api.Domain.Entities
{
    public class JobApplication : BaseEntity
    {
        public Guid CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;

        public Guid JobListingId { get; set; }
        public JobListing JobListing { get; set; } = null!;

        public Guid ResumeId { get; set; }
        public Resume Resume { get; set; } = null!;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public string? CoverLetter { get; set; }
    }
}
