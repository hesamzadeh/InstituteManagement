using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstituteManagement.Infrastructure.Data.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.SSID).IsUnique();

            builder.Property(p => p.SSID).HasMaxLength(20).IsRequired();
            builder.Property(p => p.GivenName).HasMaxLength(100).IsRequired();
            builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
            builder.Property(p => p.FathersName).HasMaxLength(100);
            builder.Property(p => p.PrimaryPhone).HasMaxLength(20);
            builder.Property(p => p.PrimaryAddress).HasMaxLength(500);
            builder.Property(p => p.PublicName).HasMaxLength(100);
            builder.Property(p => p.PasswordHash).IsRequired();

            builder.Property(p => p.SignupDate).HasDefaultValueSql("getutcdate()");
            builder.Property(p => p.TimeZone).HasDefaultValue("UTC");

            builder.HasMany(p => p.Profiles)
                   .WithOne(p => p.Person)
                   .HasForeignKey(p => p.PersonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Badges)
                   .HasConversion(
                       v => string.Join(";", v),
                       v => string.IsNullOrWhiteSpace(v) ? new List<string>() : v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

            
            builder.OwnsMany(p => p.OtherPhones, a =>
            {
                a.WithOwner().HasForeignKey("PersonId");
                a.Property(p => p.Number).HasMaxLength(20);
                a.Property(p => p.Label).HasMaxLength(50);
                a.ToTable("PersonPhoneNumbers"); // Optional: separate table
            });


            builder.OwnsMany(p => p.SocialLinks, a =>
            {
                a.WithOwner().HasForeignKey("PersonId");
                a.Property(p => p.Platform).HasMaxLength(20);
                a.Property(p => p.Url).HasMaxLength(100);
                a.ToTable("PersonSocialLinks"); // Optional: separate table
            });

            builder.OwnsOne(p => p.Email, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("Email")
                     .HasMaxLength(255)
                     .IsRequired();
            });
        }
    }
}