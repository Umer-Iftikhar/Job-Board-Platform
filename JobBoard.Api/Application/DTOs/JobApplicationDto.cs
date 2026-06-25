namespace JobBoard.Api.Application.DTOs
{
    public class JobApplicationDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedDate { get; set; }
        public string? CoverLetter { get; set; }
        public CandidateDto Candidate { get; set; } = null!;
        public JobListingSummaryDto JobListing { get; set; } = null!;
        public ResumeDto Resume { get; set; } = null!;
    }

    public class JobListingSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
