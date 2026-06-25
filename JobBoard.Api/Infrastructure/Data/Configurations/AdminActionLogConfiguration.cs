using JobBoard.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Infrastructure.Data.Configurations
{
    public class AdminActionLogConfiguration : IEntityTypeConfiguration<AdminActionLog>
    {
        public void Configure(EntityTypeBuilder<AdminActionLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AdminUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(a => a.Details)
                .HasMaxLength(2000);

            builder.HasIndex(a => a.ActionDate);
            builder.HasIndex(a => a.AdminUserId);
        }
    }
}
