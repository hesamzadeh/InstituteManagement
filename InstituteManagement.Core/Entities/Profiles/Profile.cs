using InstituteManagement.Core.Common;
using InstituteManagement.Core.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InstituteManagement.Core.Entities.Profiles
{
    public abstract class Profile : BaseEntity
    {
        [Required]
        public Guid PersonId { get; set; }

        [JsonIgnore]
        public Person Person { get; set; } = null!;

        [Required]
        public ProfileType Type { get; set; } // Shared enum, e.g., Gym, Student, etc.

        public string? DisplayName { get; set; }

        public string? NationalCode { get; set; }

        public string? Bio { get; set; }

        public bool IsVerified { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime LastLogin { get; set; }

        public DateTime? VerifiedAt { get; set; }
    }

}
