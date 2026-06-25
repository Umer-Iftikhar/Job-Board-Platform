using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Application.DTOs
{
    public class CreateJobApplicationDto
    {
        [Required]
        public Guid JobListingId { get; set; }

        [Required]
        [StringLength(200)]
        public string CandidateName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string CandidateEmail { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? CoverLetter { get; set; }
    }
}
