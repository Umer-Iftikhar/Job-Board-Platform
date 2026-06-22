using JobBoard.Api.Enums;

namespace JobBoard.Api.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int JobListingId { get; set; }
        public int CandidateId { get; set; }     // Candidate.Id
        public int? ResumeId { get; set; }       // <-- nullable, optional
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public string CoverLetter { get; set; } = string.Empty;
        public DateTime? LastUpdated { get; set; }

        public JobListing JobListing { get; set; } = new JobListing();
        public Candidate Candidate { get; set; } = new Candidate();
        public Resume Resume { get; set; } = new Resume();
    }
}
