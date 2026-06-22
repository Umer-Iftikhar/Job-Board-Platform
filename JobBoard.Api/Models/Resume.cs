namespace JobBoard.Api.Models
{
    public class Resume
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Candidate Candidate { get; set; } = new Candidate();
    }
}
