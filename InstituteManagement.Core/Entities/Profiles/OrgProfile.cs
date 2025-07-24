using InstituteManagement.Core.Common.ValueObjects;

namespace InstituteManagement.Core.Entities.Profiles
{
    public abstract class OrgProfile : Profile
    {
        public Guid? OwnerOrgProfileId { get; set; }
        public OrgProfile? OwnerOrgProfile { get; set; }
        public string? OfficialName { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public List<PhoneNumber> Phones { get; set; } = [];
        public List<SocialLink> SocialLinks { get; set; } = [];
        public List<EmailAddress> EmailAddresses { get; set; } = [];
        public List<Address> Addresses { get; set; } = [];
        public string? Description { get; set; }
        public string? ProfileCoverPhotoUrl { get; set; }
    }
}
