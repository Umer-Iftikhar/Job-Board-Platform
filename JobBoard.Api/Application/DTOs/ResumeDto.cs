namespace JobBoard.Api.Application.DTOs
{
    public class ResumeDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
