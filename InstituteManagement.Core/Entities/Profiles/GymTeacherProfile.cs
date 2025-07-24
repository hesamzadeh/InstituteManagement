
namespace InstituteManagement.Core.Entities.Profiles
{
    public class GymTeacherProfile : TeacherProfile
    {
        public bool IsIndependent { get; set; }
        public List<GymProfile> AssociatedGyms { get; set; } = [];
    }
}
