using JobBoard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Data.Configurations
{
    public class JobListingConfiguration : IEntityTypeConfiguration<JobListing>
    {
        public void Configure(EntityTypeBuilder<JobListing> builder)
        {
            builder.ToTable("JobListings");

            builder.HasOne(j => j.Employer)
                   .WithMany(e => e.JobListings)
                   .HasForeignKey(j => j.EmployerId);
        }
    }
}
