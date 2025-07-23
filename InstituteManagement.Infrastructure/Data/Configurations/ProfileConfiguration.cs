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
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Type).IsRequired();

            builder.HasDiscriminator(p => p.Type)
                   .HasValue<OrgProfile>(ProfileType.Org)
                   .HasValue<StudentProfile>(ProfileType.Student)
                   .HasValue<TeacherProfile>(ProfileType.Teacher);

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

            builder.OwnsMany(p => p.Addresses, a =>
            {
                a.WithOwner().HasForeignKey("ProfileId");
                a.Property(p => p.Line1).HasMaxLength(200);
                a.Property(p => p.Line2).HasMaxLength(200);
                a.Property(p => p.City).HasMaxLength(100);
                a.Property(p => p.Province).HasMaxLength(100);
                a.Property(p => p.PostalCode).HasMaxLength(20);
                a.Property(p => p.Country).HasMaxLength(100);
                a.ToTable("ProfileAddresses");
            });
        }
    }
}
