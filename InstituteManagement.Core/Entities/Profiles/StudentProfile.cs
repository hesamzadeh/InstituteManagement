using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Entities.Profiles
{
    public class StudentProfile : Profile
    {
        public List<OrgProfile> EnrolledOrganizations { get; set; } = [];
    }

}
