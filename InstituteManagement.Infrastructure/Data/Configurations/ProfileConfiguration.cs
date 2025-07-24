using InstituteManagement.Core.Entities.Enums;
using InstituteManagement.Core.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Infrastructure.Data.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<BaseProfile>
    {
        public void Configure(EntityTypeBuilder<BaseProfile> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Type).IsRequired();

            builder.HasDiscriminator(p => p.Type)
                   .HasValue<BaseOrgProfile>(ProfileType.)
                   .HasValue<BaseStudentProfile>(ProfileType.Student)
                   .HasValue<BaseTeacherProfile>(ProfileType.Teacher);

            builder.Property(p => p.DisplayName).HasMaxLength(255);
            builder.Property(p => p.NationalCode).HasMaxLength(20);
            builder.Property(p => p.Bio).HasMaxLength(1000);

            builder.OwnsMany(p => p.Phones, a =>
            {
                a.WithOwner().HasForeignKey("ProfileId");
                a.Property(p => p.Number).HasMaxLength(20).IsRequired();
                a.Property(p => p.Label).HasMaxLength(50);
                a.ToTable("ProfilePhones");
            });

            builder.OwnsMany(p => p.SocialLinks, a =>
            {
                a.WithOwner().HasForeignKey("ProfileId");
                a.Property(p => p.Url).HasMaxLength(255).IsRequired();
                a.Property(p => p.Platform).HasMaxLength(50);
                a.ToTable("ProfileSocialLinks");
            });

            builder.OwnsOne(p => p.Addresses, a =>
            {
                a.Property(x => x.Country).HasMaxLength(100);
                a.Property(x => x.Province).HasMaxLength(100);
                a.Property(x => x.City).HasMaxLength(100);
                a.Property(x => x.District).HasMaxLength(100);
                a.Property(x => x.FullAddress).HasMaxLength(500);
                a.Property(x => x.PostalCode).HasMaxLength(20);
                a.Property(x => x.Latitude);
                a.Property(x => x.Longitude);
            });
        }
    }
}
