using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstituteManagement.Infrastructure
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //public DbSet<Student> Students { get; set; }
    }
}
