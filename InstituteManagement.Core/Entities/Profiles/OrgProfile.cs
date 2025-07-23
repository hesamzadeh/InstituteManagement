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
        public string? ProfileCoverPhotoUrl { get; set; }
        // Relationships
        public List<TeacherProfile> Teachers { get; set; } = [];
        public List<StudentProfile> Members { get; set; } = [];
    }

}
