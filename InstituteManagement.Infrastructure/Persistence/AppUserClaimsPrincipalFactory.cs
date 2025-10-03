using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace InstituteManagement.Infrastructure.Persistence
{
    public class AppUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        private readonly IServiceProvider _serviceProvider;

        public AppUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IServiceProvider serviceProvider)
            : base(userManager, roleManager, optionsAccessor)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            // make sure 'Person' is loaded
            if (user.Person == null)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                user.Person = await db.People
                    .FirstOrDefaultAsync(p => p.AppUserId == user.Id);
            }

            var identity = await base.GenerateClaimsAsync(user);
            
            // custom claims

            if (user.Person != null)
            {
                if (!string.IsNullOrWhiteSpace(user.Person.FirstName))
                    identity.AddClaim(new Claim("FirstName", user.Person.FirstName));

                if (!string.IsNullOrWhiteSpace(user.Person.LastName))
                    identity.AddClaim(new Claim("LastName", user.Person.LastName));

                if (user.Person.LastUsedProfileId != Guid.Empty)
                    identity.AddClaim(new Claim("LastUsedProfileId", user.Person.LastUsedProfileId.ToString()));

                if (!string.IsNullOrWhiteSpace(user.Person.ProfilePictureUrl))
                    identity.AddClaim(new Claim("ProfilePictureUrl", user.Person.ProfilePictureUrl));
            }

            return identity;
        }

    }
}
