using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Entities.Profiles
{
    public class TeacherProfile : Profile
    {
        public bool IsIndependent { get; set; }
        public List<OrgProfile> AssociatedOrganizations { get; set; } = [];
    }

}
