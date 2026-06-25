namespace JobBoard.Api.Application.DTOs
{
    public class CreateJobApplicationDto
    {
        public Guid JobListingId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public string? CoverLetter { get; set; }
        // Resume file is handled separately via IFormFile in the controller
    }
}
