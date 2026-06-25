using JobBoard.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Infrastructure.Data.Configurations
{
    public class EmployerProfileConfiguration : IEntityTypeConfiguration<EmployerProfile>
    {
        public void Configure(EntityTypeBuilder<EmployerProfile> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(2000);

            builder.Property(e => e.Website)
                .HasMaxLength(500);

            builder.Property(e => e.Location)
                .HasMaxLength(200);

            builder.HasMany(e => e.JobListings)
                .WithOne(j => j.EmployerProfile)
                .HasForeignKey(j => j.EmployerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.UserId)
                .IsUnique();
        }
    }
}
