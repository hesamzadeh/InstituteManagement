using Microsoft.AspNetCore.Identity;
using System;

namespace InstituteManagement.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        // You can add custom properties here, like:
        public string FullName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
