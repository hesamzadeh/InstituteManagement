
namespace InstituteManagement.Infrastructure.Data.Configurations
{
    using InstituteManagement.Core.Entities.Profiles;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class InstituteTeacherProfileConfiguration : IEntityTypeConfiguration<InstituteTeacherProfile>
    {
        public void Configure(EntityTypeBuilder<InstituteTeacherProfile> builder)
        {
            builder.ToTable("InstituteTeacherProfiles");

            builder.Property(p => p.HireDate);
            builder.Property(p => p.ExpertiseArea).HasMaxLength(100);
        }
    }

}
