using JobBoard.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Infrastructure.Data.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.HasMany(c => c.Applications)
                .WithOne(a => a.Candidate)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Resumes)
                .WithOne(r => r.Candidate)
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
