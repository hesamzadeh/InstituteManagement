
using InstituteManagement.Shared.ValueObjects;

namespace InstituteManagement.Shared.DTOs.Profiles.OrgProfile
{
    public class OrgProfileUpdateDto
    {
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public List<PhoneNumber> Phones { get; set; } = [];
        public List<SocialLink> SocialLinks { get; set; } = [];
        public List<EmailAddress> EmailAddresses { get; set; } = [];
        public List<Address> Addresses { get; set; } = [];
        public string? Description { get; set; }
        public string? ProfileCoverPhotoUrl { get; set; }
        public string? CustomContentHtml { get; set; }

        // For admins only
        public string? CustomCss { get; set; }
    }
}
