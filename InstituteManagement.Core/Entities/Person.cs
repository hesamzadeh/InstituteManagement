using InstituteManagement.Core.Common;
using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities.Profiles;
using System.ComponentModel.DataAnnotations;

namespace InstituteManagement.Core.Entities
{
    public class Person : BaseEntity
    {
        // FK to AspNetUsers.Id (AppUser)
        public Guid? AppUserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string NationalId { get; set; } = default!;
        [Required]
        public NationalityCode NationalityCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = default!;

        public DateOnly? Birthday { get; set; }

        public List<PhoneNumber> OtherPhones { get; set; } = [];

        [MaxLength(500)]
        public Address? PrimaryAddress { get; set; }

        public DateTime SignupDate { get; set; } = DateTime.UtcNow;

        public bool IsVerified { get; set; } = false;

        public List<string> Badges { get; set; } = [];

        public List<SocialLink> SocialLinks { get; set; } = [];

        public DateTime? LastLoginTime { get; set; }

        public Guid? LastUsedProfileId { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string TimeZone { get; set; } = "UTC";

        public bool IsEnabled { get; set; } = true;

        public bool IsLocked { get; set; } = false;
        /// <summary>
        /// Indicates whether profile verification is locked (user requested verification).
        /// </summary>
        public bool IsVerificationLocked { get; set; } = false;

        /// <summary>
        /// Date/time when user requested verification (null if never requested).
        /// </summary>
        public DateTime? VerificationRequestedAt { get; set; }

        // Navigation: uploaded identity documents
        public ICollection<PersonDocument> Documents { get; set; } = new List<PersonDocument>();
        // Relationships
        public ICollection<Profile> Profiles { get; set; } = [];

    }

}
