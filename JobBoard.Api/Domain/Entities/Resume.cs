namespace JobBoard.Api.Domain.Entities
{
    public class Resume : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? ContentType { get; set; }

        public Guid CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
