using InstituteManagement.Shared.ValueObjects;

namespace InstituteManagement.Shared.DTOs.Profiles.OrgProfile
{
    public class OrgProfileDto
    {
        // From Profile
        public Guid Id { get; set; }                  // from BaseEntity
        public string? DisplayName { get; set; }
        public string? NationalCode { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime? VerifiedAt { get; set; }

        // From OrgProfile
        public Guid? OwnerOrgProfileId { get; set; }
        public string? OfficialName { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public List<PhoneNumber> Phones { get; set; } = [];
        public List<SocialLink> SocialLinks { get; set; } = [];
        public List<EmailAddress> EmailAddresses { get; set; } = [];
        public List<Address> Addresses { get; set; } = [];
        public string? Description { get; set; }
        public string? ProfileCoverPhotoUrl { get; set; }
        public string? CustomContentHtml { get; set; }

        // Only admins should see/edit this
        public string? CustomCss { get; set; }
    }
}
