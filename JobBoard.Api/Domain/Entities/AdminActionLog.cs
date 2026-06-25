namespace JobBoard.Api.Domain.Entities
{
    public class AdminActionLog : BaseEntity
    {
        public string AdminUserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; 
        public string EntityType { get; set; } = string.Empty; 
        public string EntityId { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }
}
