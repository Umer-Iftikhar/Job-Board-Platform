using JobBoard.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Infrastructure.Data.Configurations
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.AppliedDate)
                .IsRequired();

            builder.Property(a => a.CoverLetter)
                .HasMaxLength(5000);

            builder.HasOne(a => a.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.JobListing)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobListingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Resume)
                .WithMany(r => r.Applications)
                .HasForeignKey(a => a.ResumeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Matching query filter — applications for deleted jobs are hidden
            builder.HasQueryFilter(a => !a.JobListing.IsDeleted);

            builder.HasIndex(a => new { a.CandidateId, a.JobListingId })
                .IsUnique();

            builder.Property(a => a.ResumeId)
                .IsRequired();
        }
    }
}
