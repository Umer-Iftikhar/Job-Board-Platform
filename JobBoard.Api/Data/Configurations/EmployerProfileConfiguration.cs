using JobBoard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Api.Data.Configurations
{
    public class EmployerProfileConfiguration : IEntityTypeConfiguration<EmployerProfile>
    {
        public void Configure(EntityTypeBuilder<EmployerProfile> builder)
        {
            builder.ToTable("EmployerProfiles");

            // User 1 : 0..1 EmployerProfile
            builder.HasOne(e => e.User)
                   .WithOne(u => u.EmployerProfile)
                   .HasForeignKey<EmployerProfile>(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
