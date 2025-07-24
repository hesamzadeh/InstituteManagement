
namespace InstituteManagement.Core.Entities.Profiles
{
    public abstract class StudentProfile : Profile
    {
        public DateTime EnrollmentDate { get; set; }
        public string? StudentCode { get; set; }
        public string? FieldOfStudy { get; set; }
        public bool IsCurrentlyEnrolled { get; set; }
    }

}
