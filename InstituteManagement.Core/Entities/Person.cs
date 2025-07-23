using InstituteManagement.Core.Common;
using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities.Profiles;
using System.ComponentModel.DataAnnotations;

namespace InstituteManagement.Core.Entities
{
    public class Person : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string SSID { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string GivenName { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = default!;

        public DateOnly? Birthday { get; set; }

        [MaxLength(100)]
        public string? FathersName { get; set; }

        public PhoneNumber? PrimaryPhone { get; set; }

        public List<PhoneNumber> OtherPhones { get; set; } = [];

        [MaxLength(500)]
        public Address? PrimaryAddress { get; set; }

        [MaxLength(100)]
        public string? PublicName { get; set; }

        [EmailAddress]
        public EmailAddress? Email { get; set; }

        public DateTime SignupDate { get; set; } = DateTime.UtcNow;

        public bool IsVerified { get; set; } = false;

        public List<string> Badges { get; set; } = [];

        public List<SocialLink> SocialLinks { get; set; } = [];

        public string PasswordHash { get; set; } = default!;

        public DateTime? LastPasswordChange { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public Guid? LastUsedProfileId { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string TimeZone { get; set; } = "UTC";

        public bool IsEnabled { get; set; } = true;

        public bool IsLocked { get; set; } = false;
        // Relationships
        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    }

}
