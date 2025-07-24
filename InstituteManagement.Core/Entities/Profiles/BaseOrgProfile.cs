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
    public class BaseOrgProfile : BaseProfile
    {
        public Guid? OwnerOrgProfileId { get; set; }
        public BaseOrgProfile? OwnerOrgProfile { get; set; }
        public List<PhoneNumber> Phones { get; set; } = [];

        public List<SocialLink> SocialLinks { get; set; } = [];

        public List<EmailAddress> EmailAddresses { get; set; } = [];

        public List<Address> Addresses { get; set; } = [];
        public string? Description { get; set; }
        public string? ProfileCoverPhotoUrl { get; set; }
        // Relationships
        public List<BaseTeacherProfile> Teachers { get; set; } = [];
        public List<BaseStudentProfile> Members { get; set; } = [];
    }

}
