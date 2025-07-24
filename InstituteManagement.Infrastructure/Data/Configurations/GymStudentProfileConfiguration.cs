
namespace InstituteManagement.Infrastructure.Data.Configurations
{
    using InstituteManagement.Core.Entities.Profiles;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class GymStudentProfileConfiguration : IEntityTypeConfiguration<GymStudentProfile>
    {
        public void Configure(EntityTypeBuilder<GymStudentProfile> builder)
        {
            builder.ToTable("GymStudentProfiles");

            builder.Property(p => p.EnrollmentDate).IsRequired();
            builder.Property(p => p.StudentCode).HasMaxLength(100);
        }
    }

}
