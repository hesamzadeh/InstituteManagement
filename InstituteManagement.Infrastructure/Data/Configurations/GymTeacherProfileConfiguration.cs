
namespace InstituteManagement.Infrastructure.Data.Configurations
{
    using InstituteManagement.Core.Entities.Profiles;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class GymTeacherProfileConfiguration : IEntityTypeConfiguration<GymTeacherProfile>
    {
        public void Configure(EntityTypeBuilder<GymTeacherProfile> builder)
        {
            builder.ToTable("GymTeacherProfiles");

            builder.Property(p => p.HireDate);
            builder.Property(p => p.ExpertiseArea).HasMaxLength(200);
        }
    }

}
