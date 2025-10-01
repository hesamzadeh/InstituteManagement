using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace InstituteManagement.Infrastructure.Persistence
{
    public class AppUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // custom claims from Person navigation (check for null)
            identity.AddClaim(new Claim("FullName",
                !string.IsNullOrWhiteSpace(user.FullName)
                    ? user.FullName
                    : user.UserName ?? string.Empty));

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
