
namespace InstituteManagement.Core.Entities.Profiles
{
    public class InstituteStudentProfile : StudentProfile
    {

        public string? FieldOfStudy { get; set; }
        public List<InstituteProfile> EnrolledInstitutes { get; set; } = [];
    }

}
