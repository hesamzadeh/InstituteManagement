
namespace InstituteManagement.Core.Entities.Profiles
{
    public class InstituteTeacherProfile : TeacherProfile
    {
        public bool IsIndependent { get; set; }
        public List<InstituteProfile> AssociatedInstitutes { get; set; } = [];
    }
}
