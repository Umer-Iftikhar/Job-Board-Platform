namespace JobBoard.Api.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
    }
}
