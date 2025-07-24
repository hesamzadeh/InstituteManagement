
namespace InstituteManagement.Core.Entities.Profiles
{
    public class InstituteStudentProfile : StudentProfile
    {
        public List<InstituteProfile> EnrolledInstitutes { get; set; } = [];
    }

}
