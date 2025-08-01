using InstituteManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace InstituteManagement.Infrastructure
{
    public class AppUser : IdentityUser<Guid>
    {
        // You can add custom properties here, like:
        public Guid PersonId { get; set; }
        public Person? Person { get; set; }
    }
}
