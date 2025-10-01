using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities.AuditLogs;
using InstituteManagement.Core.Entities.People;
using InstituteManagement.Core.Entities.Profiles;
using InstituteManagement.Infrastructure.Data.Configurations;
using InstituteManagement.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstituteManagement.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IAppDbContext
    {
        private readonly AuditInterceptor _auditInterceptor;
        public AppDbContext(DbContextOptions<AppDbContext> options, AuditInterceptor auditInterceptor)
      : base(options)
        {
            _auditInterceptor = auditInterceptor;
        }
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
            modelBuilder.ApplyConfiguration(new PersonDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new GymProfileConfiguration());
            modelBuilder.ApplyConfiguration(new GymStudentProfileConfiguration());
            modelBuilder.ApplyConfiguration(new GymTeacherProfileConfiguration());
            modelBuilder.ApplyConfiguration(new InstituteProfileConfiguration());
            modelBuilder.ApplyConfiguration(new InstituteStudentProfileConfiguration());
            modelBuilder.ApplyConfiguration(new InstituteTeacherProfileConfiguration());

            // If you have common config logic across profiles, apply it here too
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
        }

        // Add DbSet properties only for the concrete types you will query directly
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Person> People => Set<Person>();
        public DbSet<AppRole> AppRoles => Set<AppRole>();
        public DbSet<GymProfile> GymProfiles => Set<GymProfile>();
        public DbSet<GymStudentProfile> GymStudentProfiles => Set<GymStudentProfile>();
        public DbSet<GymTeacherProfile> GymTeacherProfiles => Set<GymTeacherProfile>();
        public DbSet<InstituteProfile> InstituteProfiles => Set<InstituteProfile>();
        public DbSet<InstituteStudentProfile> InstituteStudentProfiles => Set<InstituteStudentProfile>();
        public DbSet<InstituteTeacherProfile> InstituteTeacherProfiles => Set<InstituteTeacherProfile>();
    }
}
