
namespace InstituteManagement.Infrastructure.Data.Configurations
{
    using InstituteManagement.Core.Entities.Profiles;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class InstituteProfileConfiguration : IEntityTypeConfiguration<InstituteProfile>
    {
        public void Configure(EntityTypeBuilder<InstituteProfile> builder)
        {
            builder.ToTable("InstituteProfiles");

            builder.Property(p => p.DisplayName).HasMaxLength(200).IsRequired();
            builder.Property(p => p.NationalCode).HasMaxLength(100);
            builder.Property(p => p.Website).HasMaxLength(200);
            builder.Property(p => p.VerifiedAt);

            builder.OwnsMany(p => p.EmailAddresses, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("Email")
                     .HasMaxLength(100)
                     .IsRequired();
                email.WithOwner().HasForeignKey("InstituteProfileId");
                email.ToTable("InstituteProfile_Emails");
            });

            builder.OwnsMany(p => p.Phones, a =>
            {
                a.WithOwner().HasForeignKey("OrgProfileId");
                a.Property(p => p.Number).HasMaxLength(20).IsRequired();
                a.Property(p => p.Label).HasMaxLength(50);
                a.ToTable("InstituteProfile_Phones");
            });

            builder.OwnsMany(p => p.SocialLinks, a =>
            {
                a.WithOwner().HasForeignKey("OrgProfileId");
                a.Property(s => s.Url).HasMaxLength(200).IsRequired();
                a.Property(s => s.Platform).HasMaxLength(50);
                a.ToTable("InstituteProfile_SocialLinks");
            });

            builder.OwnsMany(p => p.Addresses, a =>
            {
                a.WithOwner().HasForeignKey("OrgProfileId");
                a.Property(a => a.Country).HasMaxLength(100);
                a.Property(a => a.Province).HasMaxLength(100);
                a.Property(a => a.City).HasMaxLength(100);
                a.Property(a => a.District).HasMaxLength(100);
                a.Property(a => a.FullAddress).HasMaxLength(500);
                a.Property(a => a.PostalCode).HasMaxLength(20);
                a.ToTable("InstituteProfile_Addresses");
            });
        }
    }
}
