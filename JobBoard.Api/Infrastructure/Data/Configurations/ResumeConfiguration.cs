using JobBoard.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Infrastructure.Data.Configurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.FileName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.FileUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.ContentType)
                .HasMaxLength(100);

            builder.HasOne(r => r.Candidate)
                .WithMany(c => c.Resumes)
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Applications)
                .WithOne(a => a.Resume)
                .HasForeignKey(a => a.ResumeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
