using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Entities.Profiles
{
    public class BaseStudentProfile : BaseProfile
    {
        public List<BaseOrgProfile> EnrolledOrganizations { get; set; } = [];
    }

}
