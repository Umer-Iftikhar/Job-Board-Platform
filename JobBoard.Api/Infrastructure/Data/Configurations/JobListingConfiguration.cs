using JobBoard.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Infrastructure.Data.Configurations
{
    public class JobListingConfiguration : IEntityTypeConfiguration<JobListing>
    {
        public void Configure(EntityTypeBuilder<JobListing> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Description)
                .IsRequired();

            builder.Property(j => j.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.Experience)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.Location)
                .HasMaxLength(200);

            builder.Property(j => j.SalaryMin)
                .HasPrecision(18, 2);

            builder.Property(j => j.SalaryMax)
                .HasPrecision(18, 2);

            builder.HasOne(j => j.EmployerProfile)
                .WithMany(e => e.JobListings)
                .HasForeignKey(j => j.EmployerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.Applications)
                .WithOne(a => a.JobListing)
                .HasForeignKey(a => a.JobListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
