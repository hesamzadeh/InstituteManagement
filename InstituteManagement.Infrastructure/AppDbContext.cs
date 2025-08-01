﻿using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities;
using InstituteManagement.Core.Entities.Profiles;
using InstituteManagement.Infrastructure.Data.Configurations;
using InstituteManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstituteManagement.Infrastructure
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Explicitly tell EF that EmailAddress is an owned type
            modelBuilder.Owned<EmailAddress>();

            modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Person)
            .WithOne() // no navigation to user from Person in Core
            .HasForeignKey<AppUser>(u => u.PersonId);

            // Apply Profile Configurations

            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new GymProfileConfiguration());
            modelBuilder.ApplyConfiguration(new GymStudentProfileConfiguration());
            modelBuilder.ApplyConfiguration(new GymTeacherProfileConfiguration());
            modelBuilder.ApplyConfiguration(new InstituteProfileConfiguration());
            modelBuilder.ApplyConfiguration(new InstituteStudentProfileConfiguration());
            modelBuilder.ApplyConfiguration(new InstituteTeacherProfileConfiguration());

            // If you have common config logic across profiles, apply it here too
        }

        // Add DbSet properties only for the concrete types you will query directly
        public DbSet<Person> People => Set<Person>();
        public DbSet<GymProfile> GymProfiles => Set<GymProfile>();
        public DbSet<GymStudentProfile> GymStudentProfiles => Set<GymStudentProfile>();
        public DbSet<GymTeacherProfile> GymTeacherProfiles => Set<GymTeacherProfile>();
        public DbSet<InstituteProfile> InstituteProfiles => Set<InstituteProfile>();
        public DbSet<InstituteStudentProfile> InstituteStudentProfiles => Set<InstituteStudentProfile>();
        public DbSet<InstituteTeacherProfile> InstituteTeacherProfiles => Set<InstituteTeacherProfile>();
    }
}
