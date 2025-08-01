﻿using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace InstituteManagement.Infrastructure.Data.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.NationalId).IsUnique();
            //builder.Property(p => p.AppUserId).HasMaxLength(450);
            builder.Property(p => p.NationalId).HasMaxLength(20).IsRequired();
            builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();

            builder.Property(p => p.SignupDate).HasDefaultValueSql("getutcdate()");
            builder.Property(p => p.TimeZone).HasDefaultValue("UTC");
            builder.Property(p => p.NationalityCode).HasConversion<string>();

            builder.HasMany(p => p.Profiles)
                   .WithOne(p => p.Person)
                   .HasForeignKey(p => p.PersonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Badges)
            .HasConversion(
                v => string.Join(";", v),
                v => string.IsNullOrWhiteSpace(v) ? new List<string>() : v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));


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

            // Owned Type: PrimaryAddress
            builder.OwnsOne(p => p.PrimaryAddress, address =>
            {
                address.Property(a => a.Country)
                       .HasColumnName("Country")
                       .HasMaxLength(100);

                address.Property(a => a.Province)
                       .HasColumnName("Province")
                       .HasMaxLength(100);

                address.Property(a => a.City)
                       .HasColumnName("City")
                       .HasMaxLength(100);

                address.Property(a => a.District)
                       .HasColumnName("District")
                       .HasMaxLength(100);

                address.Property(a => a.FullAddress)
                       .HasColumnName("FullAddress")
                       .HasMaxLength(500);

                address.Property(a => a.PostalCode)
                       .HasColumnName("PostalCode")
                       .HasMaxLength(20);

                address.Property(a => a.Latitude)
                       .HasColumnName("Latitude");

                address.Property(a => a.Longitude)
                       .HasColumnName("Longitude");
            });
        }
    }
}