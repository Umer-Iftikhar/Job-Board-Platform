using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JobBoard.Api.Models;

namespace JobBoard.Api.Data.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("Applications");

            // JobListing -> Applications
            builder.HasOne(a => a.JobListing)
                   .WithMany(j => j.Applications)
                   .HasForeignKey(a => a.JobListingId);

            // Candidate -> Applications
            builder.HasOne(a => a.Candidate)
                   .WithMany(c => c.Applications)
                   .HasForeignKey(a => a.CandidateId);

            // Resume -> Applications (optional)
            builder.HasOne(a => a.Resume)
                   .WithMany()                         // no backreference from Resume
                   .HasForeignKey(a => a.ResumeId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            // Unique application per candidate per job
            builder.HasIndex(a => new { a.JobListingId, a.CandidateId })
                   .IsUnique();
        }
    }
}
