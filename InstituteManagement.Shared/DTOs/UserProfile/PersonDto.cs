using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Shared.DTOs.UserProfile
{
    public class PersonDto
    {
        public Guid Id { get; set; }
        public Guid? AppUserId { get; set; }

        public string NationalId { get; set; } = default!;
        public string NationalityCode { get; set; } = default!;

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateOnly? Birthday { get; set; }

        // Address info (flattened from value object for convenience)
        public string? Country { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? FullAddress { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public string TimeZone { get; set; } = "UTC";

        public DateTime SignupDate { get; set; }
        public DateTime? LastLoginTime { get; set; }

        public bool IsVerified { get; set; }
        public bool IsVerificationLocked { get; set; }
        public DateTime? VerificationRequestedAt { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsLocked { get; set; }
    }
}
