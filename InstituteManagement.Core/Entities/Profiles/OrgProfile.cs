using InstituteManagement.Core.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Entities.Profiles
{
    public class OrgProfile : Profile
    {
        public Guid? OwnerOrgProfileId { get; set; }
        public OrgProfile? OwnerOrgProfile { get; set; }
        public string? Description { get; set; }
        public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
        public ICollection<EmailAddress> EmailAddresses { get; set; } = new List<EmailAddress>();

        public Address? Address { get; set; }

        public string? ProfileCoverPhotoUrl { get; set; }

        // Relationships
        public List<TeacherProfile> Teachers { get; set; } = new();
        public List<StudentProfile> Members { get; set; } = new();
    }

}
