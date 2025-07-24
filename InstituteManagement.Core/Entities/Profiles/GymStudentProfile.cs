
namespace InstituteManagement.Core.Entities.Profiles
{
    public class GymStudentProfile : StudentProfile
    {
        public List<GymProfile> EnrolledGyms { get; set; } = [];
    }
}
