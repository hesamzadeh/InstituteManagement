using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Entities.Profiles
{
    public class BaseTeacherProfile : BaseProfile
    {
        public bool IsIndependent { get; set; }
        public List<BaseOrgProfile> AssociatedOrganizations { get; set; } = [];
    }

}
