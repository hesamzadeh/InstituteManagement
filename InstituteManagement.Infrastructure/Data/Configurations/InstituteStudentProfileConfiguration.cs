using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Infrastructure.Data.Configurations
{
    using InstituteManagement.Core.Entities.Profiles;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class InstituteStudentProfileConfiguration : IEntityTypeConfiguration<InstituteStudentProfile>
    {
        public void Configure(EntityTypeBuilder<InstituteStudentProfile> builder)
        {
            builder.ToTable("InstituteStudentProfiles");

            builder.Property(p => p.EnrollmentDate).IsRequired();
            builder.Property(p => p.StudentCode).HasMaxLength(100);
            builder.Property(p => p.FieldOfStudy).HasMaxLength(200);
        }
    }

}
