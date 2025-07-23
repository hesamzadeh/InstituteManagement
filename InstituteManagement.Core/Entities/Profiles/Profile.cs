using InstituteManagement.Core.Common;
using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public List<PhoneNumber> Phones { get; set; } = [];

        public List<SocialLink> SocialLinks { get; set; } = [];

        public List<EmailAddress> EmailAddresses { get; set; } = [];

        public List<Address> Addresses { get; set; } = [];

        public bool IsVerified { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime LastLogin { get; set; }

        public DateTime? VerifiedAt { get; set; }
    }

}
