
namespace InstituteManagement.Core.Entities.Profiles
{
    public abstract class TeacherProfile : Profile
    {
        public DateTime? HireDate { get; set; }
        public string? ExpertiseArea { get; set; }
    }

}
