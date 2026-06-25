namespace JobBoard.Api.Application.DTOs
{
    public class AdminActionLogDto
    {
        public Guid Id { get; set; }
        public string AdminUserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
